using System;
using MassTransit;

namespace MicroShop.Services
{
    public enum EmailType
    {
        PaymentAccepted = 0,
        PaymentRefused,
        OrderSend,
        OrderReceived,
        OrderDelayed,
        TransferReceived,
        WaitingForTransfer
    }

    public interface IEmailService
    {
        bool SendEmail(string email, EmailType type);
    }

    public class EmailSend
    {
        public string Email { get; set; }
        public int EmailType { get; set; }
    }

    public class EmailService : IEmailService
    {
        private readonly ISendEndpoint _sendEndpoint;

        public EmailService(IBus bus)
        {
            _sendEndpoint = bus.GetSendEndpoint(new Uri("rabbitmq://localhost/email_queue")).Result;
        }

        public bool SendEmail(string email, EmailType type)
        {
            _sendEndpoint.Send(new EmailSend { Email = email, EmailType = (int)type });
            return true;
        }
    }
}
