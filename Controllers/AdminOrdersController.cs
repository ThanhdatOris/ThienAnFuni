using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ThienAnFuni.Models;

namespace ThienAnFuni.Controllers
{
    public class AdminOrdersController : Controller
    {
        private readonly TAF_DbContext _context;

        public AdminOrdersController(TAF_DbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["ActiveMenu"] = "Order";
            ViewBag.Title = "Danh sách đơn hàng";

            //  0 là "pending"
            var pendingOrders = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                    .ThenInclude(d => d.Product)
                .Where(o => o.OrderStatus == (int)Helpers.ConstHelper.OrderStatus.Pending) 
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            // Truy vấn đơn hàng có trạng thái  1 là "success"
            var successOrders = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                    .ThenInclude(d => d.Product)
                .Where(o => o.OrderStatus == (int)Helpers.ConstHelper.OrderStatus.Success) 
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            ViewBag.PendingOrders = pendingOrders;
            ViewBag.SuccessOrders = successOrders;

            return View();
        }

        public async Task<IActionResult> ListDeleted()
        {
            ViewData["ActiveMenu"] = "Order";
            ViewBag.Title = "Danh sách đơn hàng";

            //  -1 là "reject"
            var rejectOrders = await _context.Orders
                .Where(o => o.OrderStatus == (int)Helpers.ConstHelper.OrderStatus.Reject)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            ViewBag.RejectOrders = rejectOrders;
            return View();
        }

        public async Task<IActionResult> Detail(int id)
        {
            ViewData["ActiveMenu"] = "Order";
            //ViewBag.Title = "Chi tiết đơn hàng";

            // Lấy chi tiết đơn hàng dựa vào orderId
            var order = await _context.Orders
                .Include(o => o.SaleStaff)
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                    .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound("Đơn hàng không tồn tại");
            }

            return View(order);
        }


    }
}