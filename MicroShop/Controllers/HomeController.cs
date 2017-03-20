using Microsoft.AspNetCore.Mvc;

namespace MicroShop.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
