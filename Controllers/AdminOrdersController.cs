using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ThienAnFuni.Models;
using ThienAnFuni.Helpers;
using ThienAnFuni.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace ThienAnFuni.Controllers
{
    [Authorize(Roles = $"{ConstHelper.RoleManager},{ConstHelper.RoleSaleStaff}")]
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

            // Lấy tất cả các đơn hàng với trạng thái liên quan
            var orders = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                    .ThenInclude(d => d.Product)
                .Where(o => o.OrderStatus == (int)ConstHelper.OrderStatus.Pending ||
                            o.OrderStatus == (int)ConstHelper.OrderStatus.Success)
                .OrderBy(o => o.OrderDate)
                .ToListAsync();

            // Phân loại đơn hàng
            var viewModel = new AdOrdersViewModel
            {
                PendingOrders = orders.Where(o => o.OrderStatus == (int)ConstHelper.OrderStatus.Pending).OrderBy(o => o.OrderDate),
                SuccessOrders = orders.Where(o => o.OrderStatus == (int)ConstHelper.OrderStatus.Success).OrderBy(o => o.OrderDate)
            };

            return View(viewModel);
        }


        public async Task<IActionResult> ListDeleted()
        {
            ViewData["ActiveMenu"] = "Order";
            ViewBag.Title = "Danh sách đơn hàng";

            //  -1 là "reject"
            var rejectOrders = await _context.Orders
                .Where(o => o.OrderStatus == (int)ConstHelper.OrderStatus.Reject)
                .Include(o => o.Customer)
                .Include(o => o.Manager)
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
                .Include(o => o.Manager)
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
        // Action with ajax
        [HttpGet]
        public IActionResult GetInfoEdit(int id)
        {
            // Tìm đơn hàng theo ID
            var order = _context.Orders.FirstOrDefault(o => o.Id == id);

            // Kiểm tra nếu đơn hàng không tồn tại
            if (order == null)
            {
                return NotFound(new { error = "Đơn hàng không tồn tại" });
            }

            // Trả về thông tin cần thiết dưới dạng JSON
            var orderInfo = new
            {
                address = order.Address,
                note = order.Note,
                paymentMethod = order.PaymentMethod
            };

            return Json(orderInfo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(int id, [FromForm] OrderUpdateModel model)
        {
            // Tìm đơn hàng theo ID
            var order = _context.Orders.FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                return NotFound(new { message = "Đơn hàng không tồn tại" });
            }

            // Cập nhật thông tin đơn hàng từ model
            order.Address = model.Address;
            order.Note = model.Note;
            order.PaymentMethod = int.Parse(model.PaymentMethod);

            // Lưu thay đổi
            _context.SaveChanges();

            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult ApproveOrder(int orderId)
        {
            try
            {
                var order = _context.Orders.FirstOrDefault(o => o.Id == orderId);
                if (order == null)
                {
                    return Json(new { success = false, message = "Đơn hàng không tồn tại." });
                }

                // Cập nhật OrderStatus, PaymentStatus, InvoiceNumber và InvoiceDate
                order.OrderStatus = (int)ConstHelper.OrderStatus.Success;
                order.PaymentStatus = (int)ConstHelper.PaymentStatus.Paid;
                order.InvoiceNumber = "HD" + DateTime.Now.ToString("yyyyMMddHHmmss") + order.CustomerId;
                order.InvoiceDate = DateTime.Now;


                var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                if (role == ConstHelper.RoleSaleStaff)
                {
                    // Nếu là SaleStaff
                    order.SaleStaffId = userId;
                    order.ManagerId = null;
                }
                else if (role == ConstHelper.RoleManager)
                {
                    // Nếu là Manager
                    order.ManagerId = userId;
                    order.SaleStaffId = null;
                }

                // Lưu thay đổi
                _context.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPut]
        public IActionResult CancelOrder(int orderId)
        {
            var order = _context.Orders.FirstOrDefault(o => o.Id == orderId);

            if (order == null)
            {
                return Json(new { success = false, message = "Đơn hàng không tồn tại!" });
            }

            // Thực hiện hành động hủy đơn hàng
            order.OrderStatus = (int)ConstHelper.OrderStatus.Reject; // Cập nhật trạng thái đơn hàng thành "Đã hủy"

            order.PaymentStatus = (int)ConstHelper.PaymentStatus.Unpaid;

            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (role == ConstHelper.RoleSaleStaff)
            {
                // Nếu là SaleStaff
                order.SaleStaffId = userId;
                order.ManagerId = null;
            }
            else if (role == ConstHelper.RoleManager)
            {
                // Nếu là Manager
                order.ManagerId = userId;
                order.SaleStaffId = null;
            }

            _context.SaveChanges();

            return Json(new { success = true });
        }


    }
}