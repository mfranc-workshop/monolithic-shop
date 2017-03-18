using System;
using System.Net.Mail;
using System.Data.Entity;
using System.Linq;
using GithubDashboard.Data;
using GithubDashboard.EmailHelpers;
using GithubDashboard.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace GithubDashboard.Controllers
{
    public class PaymentController : Controller
    {
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

            var paymentWasCreated = false;

            using (var context = new MainDatabaseContext())
            {
                var order = context.Orders
                    .Where(x => x.Id == payment.OrderId)
                    .Include(p => p.ProductOrders.Select(po => po.Product))
                    .Include(p => p.Buyer)
                    .FirstOrDefault();

                if (order == null) return View("Error");

                order.AddPayment(payment);
                order.AddBuyer(GetOrCreateNewBuyer(context, email));

                context.SaveChanges();
            }

            paymentWasCreated = ContactPaymentProvider(payment);

            SendEmail(email, paymentWasCreated);

            return paymentWasCreated ? View("Success") : View("Failure");
        }

        private Buyer GetOrCreateNewBuyer(MainDatabaseContext context, string email)
        {
            return context.Buyers.FirstOrDefault(x => x.Email == email)
                                ?? new Buyer(User.Identity.Name, User.GetEmail());
        }

        // Fake call to external payment provider
        private bool ContactPaymentProvider(Payment payment)
        {
            if(payment.Card.Number == "-1") return false;

            return true;
        }

        private void SendEmail(string email, bool success)
        {
            var smtpClient = new SmtpClient("localhost", 25);

            var mail = new MailMessage
            {
                From = new MailAddress("awesome_shop@com.pl")
            };

            mail.To.Add(new MailAddress(email));

            var emailData = Email.Templates[success ? EmailType.PaymentAccepted : EmailType.PaymentRefused];
            mail.Subject = emailData.Item1;
            mail.Body = emailData.Item2;

            try
            {
                smtpClient.Send(mail);
            }
            catch (Exception ex)
            {
                //log
            }
        }
    }
}
