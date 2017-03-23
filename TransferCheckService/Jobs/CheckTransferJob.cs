using System;
using System.Linq;
using System.Data.Entity;
using MassTransit;
using Microshop.Contract;
using NLog;
using Quartz;
using TransferCheckService.Data;

namespace TransferCheckService.Jobs
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
        void Send(string email);
    }

    public class EmailService : IEmailService
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();

        private readonly ISendEndpoint _sendEndpoint;

        public EmailService(IBus bus)
        {
            _sendEndpoint = bus.GetSendEndpoint(new Uri("rabbitmq://localhost/email_queue")).Result;
        }

        public void Send(string email)
        {
            _logger.Info($"Sheduling transfer finish email for - {email}");
            _sendEndpoint.Send(new SendEmail { Email = email, EmailType = (int)EmailType.TransferReceived });
        }
    }

    public class CheckTransferJob : IJob
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();

        private readonly ITransferCheckService _transferCheckService;
        private readonly IEmailService _emailService;

        public CheckTransferJob(IEmailService emailService, ITransferCheckService transferCheckService)
        {
            _emailService = emailService;
            _transferCheckService = transferCheckService;
        }

        public void Execute(IJobExecutionContext jobContext)
        {
            _logger.Info("TransferJob Executing");
            try
            {
                using (var context = new MainDatabaseContext())
                {
                    var orders = context.Orders
                        .Where(x => x.Status == OrderStatus.WaitingForPayment)
                        .Include(o => o.Buyer)
                        .ToList();

                    _logger.Info($"Found '{orders.Count}' waiting for the payment.");

                    foreach (var order in orders)
                    {
                        _logger.Info($"Checking transfer status for order: '{order.Id}'.");
                        var hasReceivedMoney = _transferCheckService.Check(order.Id);
                        if(hasReceivedMoney)
                        {
                            _logger.Info($"Transfer received for order: '{order.Id}'.");
                            order.TransferReceived();
                            _emailService.Send(order.Buyer.Email);
                        }
                    }

                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error when executing job");
            }

            _logger.Info("TransferJob Finished");
        }
    }
}