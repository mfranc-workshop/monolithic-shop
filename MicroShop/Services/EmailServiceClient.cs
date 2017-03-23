using System;
using MassTransit;
using Microshop.Contract;

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

    public class EmailService : IEmailService
    {
        private readonly ISendEndpoint _sendEndpoint;

        public EmailService(IBus bus)
        {
            _sendEndpoint = bus.GetSendEndpoint(new Uri("rabbitmq://localhost/email_queue")).Result;
        }

        public bool SendEmail(string email, EmailType type)
        {
            _sendEndpoint.Send(new SendEmail { Email = email, EmailType = (int)type });
            return true;
        }
    }
}
