using System;

namespace TransferCheckService.Data
{
    public enum OrderStatus
    {
        Delivering = 0,
        WaitingForWarehouse,
        WaitingForPayment,
        Blocked,
        Finished
    }

    public class Order
    {
        public Guid Id { get; set; }
        public OrderStatus Status { get; set; }
        public Buyer Buyer { get; set; }

        public Order()
        {
            Id = Guid.NewGuid();
        }

        public void TransferReceived()
        {
            this.Status = OrderStatus.WaitingForWarehouse;
        }
    }
}