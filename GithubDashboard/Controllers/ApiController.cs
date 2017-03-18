using System;
using System.Linq;
using System.Data.Entity;
using System.Net.Mail;
using GithubDashboard.Data;
using GithubDashboard.EmailHelpers;
using Microsoft.AspNetCore.Mvc;

namespace GithubDashboard.Controllers
{
    [Route("api")]
    public class OrderApi : Controller
    {
        // Get here for ease of use, normally state change shouldnt be modified by Get
        [HttpGet]
        [Route("order/{guid}/delivered")]
        public string OrderDelivered(Guid guid)
        {
            using (var context = new MainDatabaseContext())
            {
                var order = context.Orders
                    .Where(x => x.Id == guid)
                    .Include(x => x.Buyer)
                    .FirstOrDefault();

                if (order == null) return "order not found";
                if (order.Status != OrderStatus.Delivering) return "invalid order status";

                order.Delivered();

                SendEmail(order.Buyer.Email);

                context.SaveChanges();
            }

            return "order delivered";
        }

        private void SendEmail(string email)
        {
            var smtpClient = new SmtpClient("localhost", 25);

            var mail = new MailMessage
            {
                From = new MailAddress("awesome_shop@com.pl")
            };

            mail.To.Add(new MailAddress(email));

            var emailData = Email.Templates[EmailType.OrderReceived];
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
