using GithubDashboard.Services;
using Microsoft.AspNetCore.Mvc;

namespace GithubDashboard.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IReportGenerator _reportGenerator;

        public ReportsController(IReportGenerator reportGenerator)
        {
            _reportGenerator = reportGenerator;
        }

        [Route("/reports")]
        public IActionResult Index()
        {
            var report = _reportGenerator.GenerateReport();
            return View(report);
        }
    }
}
