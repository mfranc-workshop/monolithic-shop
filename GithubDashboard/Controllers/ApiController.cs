using Microsoft.AspNetCore.Mvc;

namespace GithubDashboard.Controllers
{
    public class ApiController: Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
