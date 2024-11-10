using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThienAnFuni.Models;

namespace ThienAnFuni.Controllers
{
    public class ShopController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TAF_DbContext _context;

        public ShopController(ILogger<HomeController> logger, TAF_DbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public IActionResult Index()
        {
            var products = _context.Products
                .Where(p => p.IsActive == true && p.IsImport == true)
                .ToList();
            return View(products);
        }

        public IActionResult Detail(int id)
        {
            var product = _context.Products
                .Include(p => p.Category)
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                RedirectToAction("Index");
            }

            return View(product);
        }

    }
}
