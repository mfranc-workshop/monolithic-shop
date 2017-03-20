using System;

namespace MicroShop.Services
{
    public interface ITransferCheckService
    {
        bool Check(Guid orderId);
    }

    public class TransferCheckService : ITransferCheckService
    {
        public bool Check(Guid orderId)
        {
            var rand = new Random();
            return rand.NextDouble() <= 0.8;
        }
    }
}
