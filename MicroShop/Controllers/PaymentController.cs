using System;
using System.Data.Entity;
using System.Linq;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using MicroShop.Bus;
using MicroShop.Data;
using MicroShop.EmailHelpers;
using MicroShop.Helpers;
using MicroShop.Services;

namespace MicroShop.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IEmailService _emailService;
        private readonly IPaymentProvider _paymentProvider;
        private readonly IBus _bus;

        public PaymentController(IEmailService emailService, IPaymentProvider paymentProvider, IBus bus)
        {
            _emailService = emailService;
            _paymentProvider = paymentProvider;
            _bus = bus;
        }

        [HttpGet]
        [Route("/pay/{orderId}")]
        public IActionResult Pay(Guid orderId)
        {
            return View(orderId);
        }

        [HttpPost]
        [Route("/pay/create/")]
        public IActionResult CreatePayment(Payment payment)
        {
            var email = User.GetEmail();

            var successfullPayment = false;

            using (var context = new MainDatabaseContext())
            {
                var order = context.Orders
                    .Where(x => x.Id == payment.OrderId)
                    .Include(p => p.ProductOrders.Select(po => po.Product))
                    .Include(p => p.Buyer)
                    .FirstOrDefault();

                if (order == null) return View("Error");

                order.AddBuyer(GetOrCreateNewBuyer(context, email));

                if(payment.Type == PaymentType.Card)
                {
                    successfullPayment = _paymentProvider.SendPaymentData(payment);
                    order.PayByCard(payment, successfullPayment);
                    _emailService.SendEmail(email, successfullPayment ? EmailType.PaymentAccepted : EmailType.PaymentRefused);
                    _bus.Publish<EmailSend>(new {Email = email, EmailType = successfullPayment ? EmailType.PaymentAccepted : EmailType.PaymentRefused }).Wait();
                    context.SaveChanges();
                    return successfullPayment ? View("Success") : View("Failure");
                }
                else
                {
                    _emailService.SendEmail(email, EmailType.WaitingForTransfer); 
                    _bus.Publish<EmailSend>(new {Email = email, EmailType = EmailType.WaitingForTransfer }).Wait();
                    order.PayByTransfer(payment);
                    context.SaveChanges();
                    return View("Success");
                }

            }
        }

        private Buyer GetOrCreateNewBuyer(MainDatabaseContext context, string email)
        {
            return context.Buyers.FirstOrDefault(x => x.Email == email)
                                ?? new Buyer(User.Identity.Name, User.GetEmail());
        }
    }
}
