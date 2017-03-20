using System;
using System.Net.Mail;
using MicroShop.EmailHelpers;

namespace MicroShop.Services
{
    public interface IEmailService
    {
        void SendEmail(string email, EmailType type);
    }

    public class EmailService : IEmailService
    {
        private readonly SmtpClient _client;

        public EmailService(SmtpClient client)
        {
            _client = client;
        }

        public void SendEmail(string emailAddress, EmailType type)
        {
            var mail = new MailMessage
            {
                From = new MailAddress("awesome_shop@com.pl")
            };

            mail.To.Add(new MailAddress(emailAddress));

            var emailData = Email.Templates[type];
            mail.Subject = emailData.Item1;
            mail.Body = emailData.Item2;

            try
            {
                _client.Send(mail);
            }
            catch (Exception ex)
            {
                // swallowing errors!! muahahaha
            }
        }
    }
}
