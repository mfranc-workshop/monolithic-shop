using System.Data.Entity;
using System.Linq;
using GithubDashboard.Data;
using Microsoft.AspNetCore.Mvc;

namespace GithubDashboard.Controllers
{
    public class WarehouseController : Controller
    {
        [Route("/warehouse")]
        public IActionResult Index()
        {
            using (var context = new MainDatabaseContext())
            {
                var products = context.ProductsWarehouse
                    .Include(c => c.Product)
                    .ToList();
                return View(products);
            }
        }

        [HttpPost]
        [Route("warehouse/{id}")]
        public IActionResult Update([FromForm] int numberAvailable, int id)
        {
            using (var context = new MainDatabaseContext())
            {
                var productWarehouse = context.ProductsWarehouse
                    .SingleOrDefault(x => x.Product.Id == id);

                productWarehouse.NumberAvailable = numberAvailable;

                context.SaveChanges();

                return RedirectToAction("Index");
            }
        }
    }
}