using Microsoft.AspNetCore.Mvc;

namespace GithubDashboard.Controllers
{
    public class OrderHistoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
