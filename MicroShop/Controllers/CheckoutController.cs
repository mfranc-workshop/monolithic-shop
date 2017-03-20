using System;
using System.Data.Entity;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MicroShop.Data;

namespace MicroShop.Controllers
{
    [Authorize]
    public class CheckoutController: Controller
    {
        [HttpGet]
        [Route("/checkout/{orderId}")]
        public IActionResult Index(Guid orderId)
        {
            using (var context = new MainDatabaseContext())
            {
                var order = context.Orders
                    .Where(x => x.Id == orderId)
                    .Include(p => p.ProductOrders.Select(po => po.Product))
                    .FirstOrDefault();

                return View(order);
            }
        }
    }
}