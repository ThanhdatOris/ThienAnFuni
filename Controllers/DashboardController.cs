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

            var recentSalesData = await _context.Orders
                .Where(o => o.InvoiceDate >= DateTime.Now.AddMonths(-7))
                .ToListAsync();

            var groupedSalesData = recentSalesData
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .Select(g => new SalesData
                {
                    Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                    Amount = g.Sum(o => o.TotalPrice)
                })
                .OrderBy(s => s.Date)
                .ToList();

            var totalCustomers = _context.Customers.Count();
            var totalProducts = _context.Products.Count();
            var totalOrders = _context.Orders.Count();

            //var recentOrders = await _context.Orders
            //    .OrderBy(o => o.InvoiceDate)
            //    .Take(6)
            //    .Select(o => new OrderViewModel
            //    {
            //        Id = o.Id,
            //        CustomerName = o.Customer.FullName,
            //        TotalAmount = o.TotalPrice,
            //        Status = o.OrderStatus,
            //        Date = (DateTime)o.InvoiceDate
            //    }).ToListAsync();

            var recentOrders = await  _context.Orders
                .OrderBy(o => o.InvoiceDate)
                .Take(5)
                .Select(o => new OrderViewModel
                {
                    Id = o.Id,
                    CustomerName = o.Customer.FullName ?? "",
                    TotalAmount = o.TotalPrice,
                    Status = o.OrderStatus,
                    Date = o.InvoiceDate.HasValue ? o.InvoiceDate.Value : default(DateTime)
                }).ToListAsync();

            //var lowStockProducts = await _context.Products
            //    .Join(_context.Goods,
            //          product => product.Id,
            //          goods => goods.ProductId,
            //          (product, goods) => new { product, goods })
            //    .Where(pg => pg.goods.Quantity < 10)
            //    .CountAsync();

            var lowStockProducts = await _context.Products
                .Join(_context.Goods,
                      product => product.Id,
                      goods => goods.ProductId,
                      (product, goods) => new { product, goods })
                .GroupJoin(_context.OrderDetails,
                           pg => pg.product.Id,
                           orderDetail => orderDetail.ProductId,
                           (pg, orderDetails) => new { pg.product, pg.goods, orderDetails })
                .SelectMany(pg => pg.orderDetails.DefaultIfEmpty(),
                            (pg, orderDetail) => new { pg.product, pg.goods, orderDetail })
                .GroupBy(pg => new { pg.product.Id, pg.goods.Quantity })
                .Select(g => new
                {
                    ProductId = g.Key.Id,
                    RemainingQuantity = g.Key.Quantity - g.Sum(pg => pg.orderDetail != null ? pg.orderDetail.Quantity : 0)
                })
                .Where(g => g.RemainingQuantity < 10)
                .CountAsync();



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
                RecentSalesData = groupedSalesData,
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