using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.SimpleInjectorIntegration;
using MicroShop.EmailHelpers;
using MicroShop.Services;
using SimpleInjector;

namespace MicroShop.Bus
{
    public interface EmailSend
    {
        string Email { get; }
        EmailType EmailType { get; }
    }

    public class EmailConsumer : IConsumer<EmailSend>
    {
        private readonly IEmailService _emailService;

        public EmailConsumer(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task Consume(ConsumeContext<EmailSend> context)
        {
            await Task.Run(() => _emailService.SendEmail(context.Message.Email, context.Message.EmailType));
        }
    }
}
