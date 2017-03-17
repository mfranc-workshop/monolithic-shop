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
                var products = context.Products.ToList();
                return View(products);
            }
        }

        [HttpPost]
        [Route("warehouse/{id}")]
        public IActionResult Update([FromForm] int numberAvailable, int id)
        {
            using (var context = new MainDatabaseContext())
            {
                var produt = context.Products
                    .SingleOrDefault(x => x.Id == id);

                produt.NumberAvailable = numberAvailable;

                context.SaveChanges();

                return RedirectToAction("Index");
            }
        }
    }
}