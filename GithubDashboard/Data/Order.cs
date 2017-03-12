using System;
using System.Collections.Generic;
using System.Linq;

namespace GithubDashboard.Data
{
    public class Order
    {
        public Guid Id { get; set; }
        public ICollection<ProductOrder> ProductOrders { get; set; }

        public Order()
        {
            Id = Guid.NewGuid();
        }

        public decimal Price
        {
            get
            {
                return ProductOrders.Sum(productOrder => productOrder.Count*productOrder.Product.Price);
            }
        }
    }
}