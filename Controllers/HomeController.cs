using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using ThienAnFuni.Models;
using ThienAnFuni.ViewModels;

namespace ThienAnFuni.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TAF_DbContext _context;

        public HomeController(ILogger<HomeController> logger, TAF_DbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .Where(c => c.ParentId == null && c.IsActive)
                .ToListAsync();

            var products = await _context.Products
                .Include(p => p.Category)
                .ThenInclude(c => c.ParentCategory) // Include the parent category
                .ToListAsync();

            var viewModel = new HomeViewModel
            {
                Categories = categories,
                Products = products
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
