using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MicroShop.Data;

namespace MicroShop.Controllers
{
    [Route("/order")]
    public class OrderController : Controller
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
