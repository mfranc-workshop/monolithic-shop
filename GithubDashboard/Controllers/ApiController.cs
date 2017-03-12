using System.Collections.Generic;
using System.Data.Entity;
using GithubDashboard.Data;
using Microsoft.AspNetCore.Mvc;

namespace GithubDashboard.Controllers
{
    class MainDatabaseContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<ProductOrder> ProductOrders { get; set; }
        public DbSet<Product> Products { get; set; }
    }

    [Route("api/order")]
    public class OrderApi : Controller
    {
        [HttpPut]
        public IActionResult Put([FromBody] ICollection<ProductOrder> productOrders)
        {
            var order = new Order();
            order.ProductOrders = productOrders;

            using (var context = new MainDatabaseContext())
            {
                context.Orders.Add(order);
                context.SaveChanges();
            }

            return Ok(new { orderId = order.Id });

            // send error and handle it on frontend if error display error
            // IF success redirect to checkout with ORDER GUID in URL
        }
    }
}
