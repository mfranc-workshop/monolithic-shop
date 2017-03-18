using System.Data.Entity;
using System.Linq;
using GithubDashboard.Data;
using GithubDashboard.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GithubDashboard.Controllers
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
