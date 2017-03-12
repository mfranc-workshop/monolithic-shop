using System.Collections.Generic;
using System.Linq;

namespace GithubDashboard.Data
{
    public class Order
    {
        public int Id { get; set; }
        public ICollection<ProductOrder> ProductOrders { get; set; }

        public decimal Price
        {
            get
            {
                return ProductOrders.Sum(productOrder => productOrder.Count*productOrder.Product.Price);
            }
        }
    }
}