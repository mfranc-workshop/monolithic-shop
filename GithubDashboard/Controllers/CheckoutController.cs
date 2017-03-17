using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using GithubDashboard.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GithubDashboard.Controllers
{
    public enum EmailType
    {
        PaymentAccepted,
        PaymentRefused,
        OrderSend,
        OrderReceived,
        OrderDelayed
    }

    public static class Email
    {
        public static Dictionary<EmailType, Tuple<string, string>> Templates = new Dictionary
            <EmailType, Tuple<string, string>>
        {
            { EmailType.PaymentAccepted, new Tuple<string, string>("Payment Accepted", "Thank you, your payment has been accepted. Order should be sent soon.") },
            { EmailType.PaymentRefused, new Tuple<string, string>("Payment Refused", "Sadly we couldnt accept this type of payment.") },
            { EmailType.OrderSend, new Tuple<string, string>("Your order has been sent", "Your ourder has been sent.") },
            { EmailType.OrderDelayed, new Tuple<string, string>("Your order has been delayed", "Your ourder has been delayed.") },
            { EmailType.OrderReceived, new Tuple<string, string>("Your order has been delivered", "Your ourder has been delivered.") },
        };
    }

    [Authorize]
    public class CheckoutController: Controller
    {
        [HttpGet]
        [Route("/checkout/{orderId}")]
        public IActionResult Index(Guid orderId)
        {
            using (var context = new MainDatabaseContext())
            {
                var order = context.Orders
                    .Where(x => x.Id == orderId)
                    .Include(p => p.ProductOrders.Select(po => po.Product))
                    .FirstOrDefault();

                return View(order);
            }
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
            using (var context = new MainDatabaseContext())
            {
                var order = context.Orders
                    .Where(x => x.Id == payment.OrderId)
                    .Include(p => p.ProductOrders.Select(po => po.Product))
                    .Include(p => p.Buyer)
                    .FirstOrDefault();

                payment.Price = order.Price;

                order.Payment = payment;

                var email = User.GetEmail();

                var existingBuyer = context.Buyers.FirstOrDefault(x => x.Email == email) ?? new Buyer(User.Identity.Name, User.GetEmail());

                order.Buyer = existingBuyer;

                context.SaveChanges();

                var smtpClient = new SmtpClient("localhost", 25);

                var mail = new MailMessage
                {
                    From = new MailAddress("awesome_shop@com.pl")
                };

                mail.To.Add(new MailAddress(order.Buyer.Email));

                var emailData = Email.Templates[EmailType.PaymentAccepted];
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

                return View();
            }
        }
    }
}