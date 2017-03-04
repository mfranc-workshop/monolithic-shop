using Microsoft.AspNetCore.Mvc;

namespace GithubDashboard.Controllers
{
    public class PaymentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
