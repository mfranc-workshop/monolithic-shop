using System.Collections.Generic;
using GithubDashboard.Data;
using Microsoft.AspNetCore.Mvc;

namespace GithubDashboard.Controllers
{
    [Route("api/order")]
    public class OrderApi : Controller
    {
        [HttpPut]
        public IActionResult Put([FromBody] ICollection<ProductOrder> productOrders)
        {
            var order = new Order(productOrders);

            using (var context = new MainDatabaseContext())
            {
                context.Orders.Add(order);
                context.SaveChanges();
            }

            return Ok(new { orderId = order.Id });
        }
    }
}
