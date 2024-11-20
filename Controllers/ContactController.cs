using Microsoft.AspNetCore.Mvc;

namespace ThienAnFuni.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
