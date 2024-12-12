using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ThienAnFuni.Data;
using ThienAnFuni.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ThienAnFuni.Helpers;
using Microsoft.EntityFrameworkCore;

namespace ThienAnFuni.Controllers
{

    [Authorize(Roles = $"{ConstHelper.RoleManager},{ConstHelper.RoleSaleStaff}")]

    public class DashboardController : Controller
    {
        private readonly TAF_DbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DashboardController(TAF_DbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["ActiveMenu"] = "Dashboard";

            var totalCustomers = _context.Customers.Count();
            var totalProducts = _context.Products.Count();
            var totalOrders = _context.Orders.Count();
            var lowStockProducts = _context.Products
                .Join(_context.Goods,
                      product => product.Id,
                      goods => goods.ProductId,
                      (product, goods) => new { product, goods })
                .Where(pg => pg.goods.Quantity < 10)
                .Count();

            var recentOrders = _context.Orders
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .Select(o => new OrderViewModel
                {
                    Id = o.Id,
                    CustomerName = o.Customer.FullName,
                    TotalAmount = o.TotalPrice,
                    Status = o.OrderStatus
                }).ToList();

            var newCustomers = _context.Customers
                .Join(_context.Users,
                      customer => customer.Id,
                      user => user.Id,
                      (customer, user) => new { customer, user })
                .ToList() // Fetch data from the database
                .Where(cu => _userManager.IsInRoleAsync(cu.user, "customer").Result) // Perform role check in memory
                .OrderByDescending(cu => cu.customer.IsActive)
                .Take(5)
                .Select(cu => cu.customer)
                .ToList();



            // Get the role "customer"
            var customerRole = await _roleManager.FindByNameAsync("customer");
            List<User> customerUsers = new List<User>();
            if (customerRole != null)
            {
                // Get users in the "customer" role
                customerUsers = (await _userManager.GetUsersInRoleAsync(customerRole.Name)).ToList();
            }

            var viewModel = new DashboardViewModel
            {
                TotalCustomers = totalCustomers,
                TotalProducts = totalProducts,
                TotalOrders = totalOrders,
                LowStockProducts = lowStockProducts,
                RecentOrders = recentOrders,
                NewCustomers = newCustomers,
                CustomerUsers = customerUsers
            };

            return View(viewModel);
        }
    }
}