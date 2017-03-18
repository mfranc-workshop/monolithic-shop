using System.Threading;
using Microsoft.AspNetCore.Mvc;

namespace GithubDashboard.Controllers
{
    public class ReportsController : Controller
    {
        [Route("/reports")]
        public IActionResult Index()
        {
            Thread.Sleep(2000);
            return View();
        }
    }
}
