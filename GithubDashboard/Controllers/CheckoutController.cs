using System.Data.Entity;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace GithubDashboard.Controllers
{
    public class CheckoutController: Controller
    {
        [HttpGet]
        [Route("/checkout/{orderId}")]
        public IActionResult Index(int orderId)
        {
            //handle order not found etc
            using (var context = new MainDatabaseContext())
            {
                var order = context.Orders
                    .Where(x => x.Id == orderId)
                    .Include(p => p.ProductOrders.Select(po => po.Product))
                    .FirstOrDefault();

                return View(order);
            }
            // OOPS ERROR 
        }
    }
}
