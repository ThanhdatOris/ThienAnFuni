using Microsoft.AspNetCore.Mvc;
using ThienAnFuni.Models;

namespace ThienAnFuni.Controllers
{
    public class POSController : Controller
    {
        private readonly TAF_DbContext _context;

        public POSController(TAF_DbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
