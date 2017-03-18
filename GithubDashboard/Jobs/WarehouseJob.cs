using System;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using GithubDashboard.Data;
using GithubDashboard.EmailHelpers;
using Quartz;

namespace GithubDashboard.Jobs
{
    public class WarehouseJob : IJob
    {
        public void Execute(IJobExecutionContext jobContext)
        {
            using (var context = new MainDatabaseContext())
            {
                var orders = context.Orders
                    .Where(x => x.Status == OrderStatus.WaitingForWarehouse)
                    .Include(o => o.Buyer)
                    .Include(o => o.ProductOrders.Select(p => p.Product))
                    .ToList();

                foreach (var order in orders)
                {
                    var cannotFulfill = (from productOrder in order.ProductOrders
                        let productWarehouse = context.ProductsWarehouse.FirstOrDefault(pw => pw.Product.Id == productOrder.ProductId)
                        where productWarehouse.NumberAvailable < productOrder.Count
                        select productOrder).Any();

                    if (cannotFulfill)
                    {
                        break;
                    }
                    else
                    {
                        foreach (var productOrder in order.ProductOrders)
                        {
                            var productW = context.ProductsWarehouse
                                .Include(pw => pw.Product)
                                .FirstOrDefault(x => x.Product.Id == productOrder.ProductId);

                            productW.NumberAvailable = productW.NumberAvailable - productOrder.Count;
                        }

                        SendEmail(order.Buyer.Email);
                        order.Delivering();
                    }
                }

                context.SaveChanges();
            }
        }

        private void SendEmail(string email)
        {
            var smtpClient = new SmtpClient("localhost", 25);

            var mail = new MailMessage
            {
                From = new MailAddress("awesome_shop@com.pl")
            };

            mail.To.Add(new MailAddress(email));

            var emailData = Email.Templates[EmailType.OrderSend];
            mail.Subject = emailData.Item1;
            mail.Body = emailData.Item2;

            try
            {
                smtpClient.Send(mail);
            }
            catch (Exception ex)
            {
                // swallowing errors!! muahahaha
            }
        }
    }
}