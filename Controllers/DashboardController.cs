using Microsoft.AspNetCore.Mvc;

namespace ThienAnFuni.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            ViewData["ActiveMenu"] = "Dashboard";

            return View();
        }
    }
}
