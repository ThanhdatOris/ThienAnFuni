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

            var featuredProducts = await _context.Products
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.Price)
                .Take(6)
                .ToListAsync();

            var newProducts = await _context.Products
                .OrderByDescending(p => p.CreatedDate)
                .Take(6)
                .ToListAsync();

            var bestSellerProducts = await _context.Products
                .Join(_context.OrderDetails,
                    product => product.Id,
                    orderDetail => orderDetail.ProductId,
                    (product, orderDetail) => new { product, orderDetail })
                .GroupBy(po => po.product)
                .Select(g => new
                {
                    Product = g.Key,
                    TotalSold = g.Sum(po => po.orderDetail.Quantity)
                })
                .OrderByDescending(g => g.TotalSold)
                .Take(6)
                .Select(g => g.Product)
                .ToListAsync();

            var viewModel = new HomeViewModel
            {
                Categories = categories,
                Products = products,
                FeaturedProducts = featuredProducts,
                NewProducts = newProducts,
                BestSellerProducts = bestSellerProducts
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
