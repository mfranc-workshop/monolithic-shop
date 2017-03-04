using Microsoft.AspNetCore.Mvc;

namespace GithubDashboard.Controllers
{
    public class ReportsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
