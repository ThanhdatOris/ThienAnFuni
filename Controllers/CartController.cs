using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThienAnFuni.Extensions;
using SessionExtensions = ThienAnFuni.Extensions.SessionExtensions;
using ThienAnFuni.Models;
using ThienAnFuni.ViewModels;
using Microsoft.AspNetCore.HttpLogging;
using System.Security.Claims;
using ThienAnFuni.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

using ThienAnFuni.Services;
namespace ThienAnFuni.Controllers
{
    public class CartController : Controller
    {
        private readonly IEmailSender _emailSender;
        private readonly TAF_DbContext _context;
        private readonly UserManager<User> _userManager;

        public CartController(TAF_DbContext context, UserManager<User> userManager, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }
        public IActionResult Index()
        {
            // Lấy giỏ hàng từ session
            var cart = HttpContext.Session.GetObjectFromJson<Dictionary<int, CartDetail>>("cart") ?? new Dictionary<int, CartDetail>();

            // Tính tổng tiền của giỏ hàng
            decimal total = cart.Sum(item => (decimal)item.Value.Price * item.Value.Quantity);

            //// Giả sử VAT là 10%
            //decimal vat = total * 0.10m;

            //// Tính tổng giá đã bao gồm VAT
            //decimal totalWithVAT = total + vat;

            // Lưu vào model
            var model = new CartViewModel
            {
                CartItems = cart,
                Total = total // Tổng giá đã bao gồm VAT
            };

            // Trả về View
            return View(model);
        }



        [HttpPost]
        public async Task<IActionResult> AddToCart(int id, int quantity = 1)
        {
            var product = await _context.Products
                .Where(p => p.Id == id && p.IsActive)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.MainImg,
                    p.IsActive,
                    p.Price,
                    CategoryName = p.Category.Name,
                    QuantityInStock = (_context.Goods
                        .Where(g => g.ProductId == p.Id)
                        .Sum(g => (int?)g.Quantity) ?? 0) -
                        (_context.OrderDetails
                        .Where(o => o.ProductId == p.Id)
                        .Sum(o => (int?)o.Quantity) ?? 0)
                })
                .FirstOrDefaultAsync();

            if (product == null || product.QuantityInStock <= 0)
            {
                return BadRequest(new { error = "Sản phẩm này hiện đã hết hàng!" });
            }

            var cart = HttpContext.Session.GetObjectFromJson<Dictionary<int, CartDetail>>("cart") ?? new Dictionary<int, CartDetail>();

            // Kiểm tra xem tổng số lượng muốn thêm có vượt quá tồn kho không
            if (product.QuantityInStock < (cart.GetValueOrDefault(id)?.Quantity ?? 0) + quantity)
            {
                //return Json(new { error = "Thêm sản phẩm quá số lượng trong kho!" });
                return BadRequest(new { error = "Thêm sản phẩm quá số lượng trong kho!" });

            }

            // Nếu sản phẩm đã có trong giỏ hàng, tăng số lượng
            if (cart.ContainsKey(id))
            {
                cart[id].Quantity += quantity;
            }
            else
            {
                // Nếu chưa có trong giỏ hàng, thêm sản phẩm vào giỏ
                cart[id] = new CartDetail
                {
                    ProductId = product.Id,
                    Product = new Product { Id = product.Id, Name = product.Name, Price = product.Price, MainImg = product.MainImg },
                    Price = product.Price,
                    Quantity = quantity,
                };
            }

            // Cập nhật lại giỏ hàng vào session
            HttpContext.Session.SetObjectAsJson("cart", cart);

            // Tính tổng tiền và tổng số lượng
            decimal total = cart.Sum(item => (decimal)(item.Value.Price * item.Value.Quantity));
            int totalQuantity = cart.Sum(item => item.Value.Quantity);

            // Cập nhật tổng tiền và số lượng vào session
            HttpContext.Session.SetDecimal("total", total);
            HttpContext.Session.SetCustomInt32("totalQuantity", totalQuantity);

            return Json(new
            {
                message = "Sản phẩm đã được thêm vào giỏ hàng!",
                total = total.ToString("C0", new System.Globalization.CultureInfo("vi-VN")),
                totalQuantity
            });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCart(int id, int quantity)
        {
            // Lấy giỏ hàng từ session
            var cart = HttpContext.Session.GetObjectFromJson<Dictionary<int, CartDetail>>("cart") ?? new Dictionary<int, CartDetail>();

            // Lấy sản phẩm từ cơ sở dữ liệu
            var product = await _context.Products
                .Where(p => p.Id == id && p.IsActive == true && p.IsImport == true)
                .Include(p => p.Category)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.MainImg,
                    p.IsActive,
                    p.Price,
                    CategoryName = p.Category.Name,
                    QuantityInStock = (_context.Goods
                        .Where(g => g.ProductId == p.Id)
                        .Sum(g => (int?)g.Quantity) ?? 0) -
                        (_context.OrderDetails
                        .Where(o => o.ProductId == p.Id)
                        .Sum(o => (int?)o.Quantity) ?? 0)
                })
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return Json(new { error = "Sản phẩm không tồn tại" });
            }

            // Kiểm tra xem sản phẩm có tồn kho không
            if (product.QuantityInStock <= 0)
            {
                return Json(new { error = "Sản phẩm này hiện đã hết hàng!" });
            }

            // Kiểm tra số lượng muốn cập nhật
            if (quantity > product.QuantityInStock)
            {
                return Json(new { error = "Số lượng vượt quá tồn kho" });
            }

            // Kiểm tra xem sản phẩm có trong giỏ hàng không
            if (cart.ContainsKey(id))
            {
                // Cập nhật số lượng sản phẩm trong giỏ hàng
                cart[id].Quantity = quantity;

                // Cập nhật lại session giỏ hàng
                HttpContext.Session.SetObjectAsJson("cart", cart);

                // Tính lại tổng giá của sản phẩm
                decimal productTotal = (decimal)cart[id].Price * cart[id].Quantity;

                // Tính lại tổng giá và tổng số lượng của giỏ hàng
                decimal total = cart.Sum(item => (decimal)item.Value.Price * item.Value.Quantity);
                int totalQuantity = cart.Sum(item => item.Value.Quantity);

                // Cập nhật lại session với tổng tiền và tổng số lượng
                HttpContext.Session.SetDecimal("total", total);
                HttpContext.Session.SetCustomInt32("totalQuantity", totalQuantity);

                // Trả về kết quả
                return Json(new
                {
                    productTotal = productTotal.ToString("C0", new System.Globalization.CultureInfo("vi-VN")),
                    total = total.ToString("C0", new System.Globalization.CultureInfo("vi-VN")),
                    totalQuantity
                });
            }

            return Json(new { error = "Sản phẩm không tồn tại trong giỏ hàng" });
        }

        [HttpPost]
        public IActionResult RemoveProSession(int id)
        {
            var cart = HttpContext.Session.GetObjectFromJson<Dictionary<int, CartDetail>>("cart") ?? new Dictionary<int, CartDetail>();

            if (cart.ContainsKey(id))
            {
                cart.Remove(id);

                // Cập nhật lại giỏ hàng trong session
                HttpContext.Session.SetObjectAsJson("cart", cart);

                decimal total = cart.Sum(item => (decimal)item.Value.Price * item.Value.Quantity);
                int totalQuantity = cart.Sum(item => item.Value.Quantity);

                // Cập nhật lại session với tổng tiền và tổng số lượng
                HttpContext.Session.SetDecimal("total", total);
                HttpContext.Session.SetCustomInt32("totalQuantity", totalQuantity);
            }

            // Quay lại trang giỏ hàng sau khi xóa
            return RedirectToAction("Index");
        }

        [Authorize(Roles = $"{ConstHelper.RoleCustomer}")]

        //public IActionResult CheckOutPro()
        //{
        //    // Lấy giỏ hàng từ session
        //    var cart = HttpContext.Session.GetObjectFromJson<Dictionary<int, CartDetail>>("cart") ?? new Dictionary<int, CartDetail>();

        //    // Lấy tổng tiền từ session
        //    var total = HttpContext.Session.GetDecimal("total");

        //    // Tạo model hoặc sử dụng ViewBag để truyền dữ liệu
        //    ViewBag.Cart = cart;
        //    ViewBag.Total = total;

        //    return View();
        //}
        public IActionResult CheckOutPro()
        {
            // Lấy giỏ hàng từ session
            var cart = HttpContext.Session.GetObjectFromJson<Dictionary<int, CartDetail>>("cart") ?? new Dictionary<int, CartDetail>();

            // Lấy tổng tiền từ session
            var total = HttpContext.Session.GetDecimal("total");

            // Giả sử UserId là khóa ngoại trong bảng Orders
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Lấy ID người dùng hiện tại
            var lastOrder = _context.Orders
                                    .Where(o => o.CustomerId == userId)
                                    .OrderByDescending(o => o.OrderDate) // Lấy đơn hàng mới nhất
                                    .FirstOrDefault();

            var lastAddress = lastOrder?.Address ?? ""; // Lấy địa chỉ từ đơn hàng hoặc để trống

            // Truyền dữ liệu qua ViewBag hoặc thêm vào model
            ViewBag.LastAddress = lastAddress;
            ViewBag.Cart = cart;
            ViewBag.Total = total;

            return View();
        }



        [HttpPost]
        #region Old Version CheckOutSV
        //public async Task<IActionResult> CheckOutSV(string address, int paymentMethod, string? note)
        //{
        //    // Lấy cart từ session
        //    var cart = HttpContext.Session.GetObjectFromJson<Dictionary<int, CartDetail>>("cart");

        //    if (cart == null || cart.Count() < 0)
        //    {
        //        TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống. Vui lòng thêm sản phẩm trước khi thanh toán.";
        //        return RedirectToAction("Index", "Cart");
        //    }

        //    if (string.IsNullOrWhiteSpace(address))
        //    {
        //        TempData["ErrorMessage"] = "Vui lòng nhập địa chỉ giao hàng.";
        //        return RedirectToAction("CheckOutPro", "Cart");
        //    }

        //    // Kiểm tra phương thức thanh toán có hợp lệ không
        //    if (!Enum.IsDefined(typeof(ConstHelper.PaymentMethod), paymentMethod))
        //    {
        //        TempData["ErrorMessage"] = "Phương thức thanh toán không hợp lệ.";
        //        return RedirectToAction("CheckOutPro", "Cart");
        //    }

        //    string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var user = await _userManager.FindByIdAsync(userId);

        //    if (user == null)
        //    {
        //        return Unauthorized("Không tìm thấy người dùng hiện tại.");
        //    }

        //    // Lấy danh sách các role của người dùng hiện tại
        //    var userRoles = await _userManager.GetRolesAsync(user);

        //    // Khởi tạo ID mặc định cho các vai trò
        //    string? customerId = null;
        //    string? saleStaffId = null;
        //    string? managerId = null;

        //    // Xác định ID của các vai trò
        //    if (userRoles.Contains(ConstHelper.RoleCustomer))
        //    {
        //        customerId = userId;  // Người dùng là Customer
        //    }

        //    if (userRoles.Contains(ConstHelper.RoleSaleStaff))
        //    {
        //        saleStaffId = userId;  // Người dùng là SaleStaff
        //    }

        //    if (userRoles.Contains(ConstHelper.RoleManager))
        //    {
        //        managerId = userId;  // Người dùng là Manager
        //    }

        //    using (var transaction = await _context.Database.BeginTransactionAsync())
        //    {
        //        try
        //        {
        //            // Đơn hàng
        //            var order = new Order
        //            {
        //                CustomerId = customerId,
        //                SaleStaffId = saleStaffId,
        //                ManagerId = managerId,
        //                Address = address,
        //                TotalQuantity = cart.Sum(item => item.Value.Quantity),
        //                TotalPrice = cart.Sum(item => (double)item.Value.Price * item.Value.Quantity),
        //                OrderDate = DateTime.Now,
        //                OrderStatus = (int)ConstHelper.OrderStatus.Pending,
        //                PaymentMethod = paymentMethod,
        //                Note = note,
        //                CustomerPhoneNumber = user.PhoneNumber
        //            };

        //            _context.Orders.Add(order);
        //            await _context.SaveChangesAsync();

        //            // Chi tiết đơn hàng
        //            foreach (var item in cart)
        //            {
        //                // Should: SP có tồn tại trong DB không
        //                var product = await _context.Products.FindAsync(item.Key);
        //                if (product == null)
        //                {
        //                    TempData["ErrorMessage"] = $"Sản phẩm với ID {item.Key} không tồn tại.";
        //                    return RedirectToAction("Index", "Cart");
        //                }

        //                var orderDetail = new OrderDetail
        //                {
        //                    OrderId = order.Id,
        //                    ProductId = item.Key,
        //                    Quantity = item.Value.Quantity,
        //                    PriceAtOrder = item.Value.Price
        //                };

        //                _context.OrderDetails.Add(orderDetail);
        //            }

        //            await _context.SaveChangesAsync();

        //            // Commit transaction
        //            await transaction.CommitAsync();

        //            // Xóa giỏ hàng
        //            HttpContext.Session.Remove("cart");
        //            HttpContext.Session.Remove("total");
        //            HttpContext.Session.Remove("totalQuantity");

        //            // Send email
        //            if (order != null)
        //            {
        //                string subject = "Đặt Hàng Thành Công - Thiên Ân Store";
        //                string message = $@"
        //                <h2>Cảm ơn bạn đã đặt hàng tại Thiên Ân Store!</h2>
        //                <p>Đơn hàng #{order.Id} đã được tạo thành công.</p>
        //                <p>Địa chỉ giao hàng: {order.Address}</p>
        //                <p>Tổng số lượng: {order.TotalQuantity}</p>
        //                <p>Tổng giá: {order.TotalPrice:n0}đ</p>
        //                <p>Chúng tôi sẽ liên hệ với bạn sớm nhất để giao hàng.</p>";

        //                await _emailSender.SendEmailAsync(user.Email, subject, message);
        //            }


        //            // Thông báo thành công
        //            return RedirectToAction("OrderSuccess", "Orders");
        //        }
        //        catch (Exception ex)
        //        {
        //            // Rollback transaction nếu xảy ra lỗi
        //            await transaction.RollbackAsync();

        //            // Log lỗi (nếu có hệ thống logging)
        //            Console.WriteLine(ex.Message);

        //            TempData["ErrorMessage"] = "Đã xảy ra lỗi khi xử lý đơn hàng. Vui lòng thử lại sau.";
        //            return RedirectToAction("Index", "Cart");
        //        }
        //    }
        //}
        #endregion

        public async Task<IActionResult> CheckOutSV(string address, int paymentMethod, string? note)
        {
            var cart = HttpContext.Session.GetObjectFromJson<Dictionary<int, CartDetail>>("cart");

            if (cart == null || cart.Count <= 0)
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống. Vui lòng thêm sản phẩm trước khi thanh toán.";
                return RedirectToAction("Index", "Cart");
            }

            if (string.IsNullOrWhiteSpace(address))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập địa chỉ giao hàng.";
                return RedirectToAction("CheckOutPro", "Cart");
            }

            if (!Enum.IsDefined(typeof(ConstHelper.PaymentMethod), paymentMethod))
            {
                TempData["ErrorMessage"] = "Phương thức thanh toán không hợp lệ.";
                return RedirectToAction("CheckOutPro", "Cart");
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) return Unauthorized("Không tìm thấy người dùng hiện tại.");

            var userRoles = await _userManager.GetRolesAsync(user);

            string? customerId = null, saleStaffId = null, managerId = null;
            if (userRoles.Contains(ConstHelper.RoleCustomer)) customerId = userId;
            if (userRoles.Contains(ConstHelper.RoleSaleStaff)) saleStaffId = userId;
            if (userRoles.Contains(ConstHelper.RoleManager)) managerId = userId;

            Order? order = null;

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Tạo đơn hàng
                    order = new Order
                    {
                        CustomerId = customerId,
                        SaleStaffId = saleStaffId,
                        ManagerId = managerId,
                        Address = address,
                        TotalQuantity = cart.Sum(item => item.Value.Quantity),
                        TotalPrice = cart.Sum(item => item.Value.Price * item.Value.Quantity),
                        OrderDate = DateTime.Now,
                        OrderStatus = (int)ConstHelper.OrderStatus.Pending,
                        PaymentMethod = paymentMethod,
                        Note = note,
                        CustomerPhoneNumber = user.PhoneNumber
                    };

                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync();

                    // Thêm chi tiết đơn hàng
                    foreach (var item in cart)
                    {
                        var product = await _context.Products.FindAsync(item.Key);
                        if (product == null)
                        {
                            TempData["ErrorMessage"] = $"Sản phẩm với ID {item.Key} không tồn tại.";
                            return RedirectToAction("Index", "Cart");
                        }

                        var orderDetail = new OrderDetail
                        {
                            OrderId = order.Id,
                            ProductId = item.Key,
                            Quantity = item.Value.Quantity,
                            PriceAtOrder = item.Value.Price
                        };

                        _context.OrderDetails.Add(orderDetail);
                    }

                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine(ex.Message);
                    TempData["ErrorMessage"] = "Đã xảy ra lỗi khi xử lý đơn hàng. Vui lòng thử lại sau.";
                    return RedirectToAction("Index", "Cart");
                }
            }

            // Xóa giỏ hàng sau khi hoàn tất transaction
            HttpContext.Session.Remove("cart");
            HttpContext.Session.Remove("total");
            HttpContext.Session.Remove("totalQuantity");

            // Gửi email sau khi transaction hoàn tất
            if (order != null)
            {
                string subject = "💕💕💕 Đặt Hàng Thành Công - Thiên Ân Store 💕💕💕";
                string message = $@"
                <h2>💌Cảm ơn bạn đã đặt hàng tại Thiên Ân Store!💌</h2>
                <p>🎁 Đơn hàng #{order.Id} đã được tạo thành công.</p>
                <p>🎁 Địa chỉ giao hàng: {order.Address}</p>
                <p>🎁 Tổng số lượng: {order.TotalQuantity}</p>
                <p>🎁 Tổng giá: {order.TotalPrice:n0}đ</p>
                <p>Chúng tôi sẽ liên hệ với bạn sớm nhất để giao hàng ❤️.</p>";

                await _emailSender.SendEmailAsync(user.Email, subject, message);
            }

            //TempData["SuccessMessage"] = "Đặt hàng thành công!";
            return RedirectToAction("OrderSuccess", "Orders");
        }


    }
}
