using Microsoft.AspNetCore.Mvc;

namespace GithubDashboard.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
