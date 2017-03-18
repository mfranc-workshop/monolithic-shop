using System;
using System.Collections.Generic;
using System.Linq;

namespace GithubDashboard.Data
{
    public enum OrderStatus
    {
        Pending = 0,
        PendingPayment,
        Delivering,
        Finished
    }

    public class Order
    {
        public Guid Id { get; set; }
        public OrderStatus Status { get; set; }
        public Buyer Buyer { get; set; }
        public Payment Payment { get; set; }
        public ICollection<ProductOrder> ProductOrders { get; set; }

        public Order()
        {
            Id = Guid.NewGuid();
            Status = OrderStatus.Pending;
        }

        public Order(ICollection<ProductOrder> productOrders)
            : this()
        {
            ProductOrders = productOrders;
        }

        public decimal Price
        {
            get
            {
                return ProductOrders.Sum(productOrder => productOrder.Count*productOrder.Product.Price);
            }
        }

        public void AddPayment(Payment payment)
        {
            payment.Price = this.Price;
            this.Payment = payment;
        }

        public void AddBuyer(Buyer buyer)
        {
            this.Buyer = buyer;
        }
    }
}