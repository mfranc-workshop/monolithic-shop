using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MicroShop.Data;

namespace MicroShop.Controllers
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
