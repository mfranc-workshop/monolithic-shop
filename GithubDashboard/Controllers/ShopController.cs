using System.Linq;
using GithubDashboard.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GithubDashboard.Controllers
{
    [Authorize]
    public class ShopController: Controller
    {
        public IActionResult Index()
        {
            using (var context = new MainDatabaseContext())
            {
                var products = context.Products.ToList();
                return View(products);
            }
        }
    }
}
