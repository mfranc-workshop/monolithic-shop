using Microsoft.AspNetCore.Mvc;

namespace GithubDashboard.Controllers
{
    public class CartController: Controller
    {
        [HttpPost]
        public IActionResult Add()
        {
            return View();
        }
    }
}
