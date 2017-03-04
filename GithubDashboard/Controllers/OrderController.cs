using Microsoft.AspNetCore.Mvc;

namespace GithubDashboard.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
