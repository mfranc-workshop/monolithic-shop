using System;
using System.Linq;
using System.Data.Entity;
using NLog;
using Quartz;
using TransferCheckService.Data;

namespace TransferCheckService.Jobs
{
    public class CheckTransferJob : IJob
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();

        private readonly ITransferCheckService _transferCheckService;

        public CheckTransferJob()
        {
            _transferCheckService = new TransferCheckService();
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
                            //_emailService.SendEmail(order.Buyer.Email, EmailType.TransferReceived);
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