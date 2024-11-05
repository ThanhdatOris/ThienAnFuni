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

            // Truy vấn đơn hàng có trạng thái "pending" (giả sử 0 là "pending")
            var pendingOrders = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                    .ThenInclude(d => d.Product)
                .Where(o => o.OrderStatus == (int)Helpers.ConstHelper.OrderStatus.Pending) // Điều kiện trạng thái đơn hàng là "pending"
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            // Truy vấn đơn hàng có trạng thái "success" (giả sử 1 là "success")
            var successOrders = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                    .ThenInclude(d => d.Product)
                .Where(o => o.OrderStatus == (int)Helpers.ConstHelper.OrderStatus.Success) // Điều kiện trạng thái đơn hàng là "success"
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            // Truyền dữ liệu vào ViewBag hoặc dùng ViewModel để gửi đến View
            ViewBag.Title = "Danh sách đơn hàng";
            ViewBag.PendingOrders = pendingOrders;
            ViewBag.SuccessOrders = successOrders;

            return View();
        }


    }
}