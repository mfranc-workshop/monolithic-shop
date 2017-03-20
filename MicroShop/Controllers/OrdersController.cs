using System.Data.Entity;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MicroShop.Data;
using MicroShop.Helpers;

namespace MicroShop.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        [Route("/orders")]
        public IActionResult Index()
        {
            using (var context = new MainDatabaseContext())
            {
                var email = User.GetEmail();

                var orders = context.Orders
                    .Where(x => x.Buyer.Email == email)
                    .Include(x => x.Buyer)
                    .Include(x => x.Payment)
                    .Include(x => x.ProductOrders.Select(po => po.Product))
                    .ToList();

                return View(orders);
            }
        }
    }
}
