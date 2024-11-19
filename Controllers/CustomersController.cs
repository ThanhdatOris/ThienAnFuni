using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThienAnFuni.Models;

namespace ThienAnFuni.Controllers
{
    public class CustomersController : Controller
    {
        private readonly TAF_DbContext _context;

        public CustomersController(TAF_DbContext context)
        {
            _context = context;
        }
        // GET: Customers
        public async Task<IActionResult> Index()
        {
            ViewData["ActiveMenu"] = "Customer";

            return View(await _context.Customers.Where(s => s.IsActive == true).ToListAsync());
        }

    }
}
