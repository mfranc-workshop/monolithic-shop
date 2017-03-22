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
        public EmailType Type { get; set; }
    }

    public class EmailService : IEmailService
    {
        private readonly IBus _bus;

        public EmailService(IBus bus)
        {
            _bus = bus;
        }

        public bool SendEmail(string email, EmailType type)
        {
            _bus.Publish<EmailSend>(new { Email = email, Type = type });
            return true;
        }
    }
}
