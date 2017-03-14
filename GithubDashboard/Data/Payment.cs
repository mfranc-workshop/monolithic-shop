using System;

namespace GithubDashboard.Data
{
    public enum PaymentStatus
    {
        Scheduled = 0,
        Processed,
        Declined
    }

    public class Card
    {
        public string Number { get; set; }
    }

    public class Payment
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public string Address { get; set; }
        public Guid OrderId { get; set; }
        public Card Card { get; set; }
        public PaymentType Type { get; set; }
        public PaymentStatus Status { get; set; }

        public Payment()
        {
            Status = PaymentStatus.Scheduled;
        }

        public Payment(Guid orderId)
            : this()
        {
            OrderId = orderId;
        }
    }
}