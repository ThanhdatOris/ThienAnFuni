using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ThienAnFuni.Helpers;
using ThienAnFuni.Models;

namespace ThienAnFuni.Controllers
{
    public class POSController : Controller
    {
        private readonly TAF_DbContext _context;

        public POSController(TAF_DbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.ToListAsync();
            var cart = HttpContext.Session.GetString("cart");
            var cartItems = string.IsNullOrEmpty(cart) ? new List<CartDetail>() : JsonConvert.DeserializeObject<List<CartDetail>>(cart);

            ViewBag.Title = "POS Bán hàng";
            ViewBag.Cart = cartItems;

            return View(products);
        }

        public async Task<IActionResult> AddProductSession(int id, int productListQuantity = 1)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return Json(new { message = "Sản phẩm không tồn tại!" });
            }

            var cart = HttpContext.Session.GetString("cart");
            var cartItems = string.IsNullOrEmpty(cart) ? new List<CartDetail>() : JsonConvert.DeserializeObject<List<CartDetail>>(cart);

            var existingProduct = cartItems.FirstOrDefault(item => item.ProductId == id);

            if (existingProduct != null)
            {
                existingProduct.Quantity += productListQuantity;
            }
            else
            {
                cartItems.Add(new CartDetail
                {
                    ProductId = product.Id,
                    Product = product,
                    Price = product.Price,
                    Quantity = productListQuantity,
                    //ImageUrl = product.MainImage?.ImageUrl ?? "/images/default.jpg"
                });
            }

            // Lưu giỏ hàng vào session
            HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(cartItems));

            // Tính tổng tiền và số lượng
            var total = cartItems.Sum(item => item.Price * item.Quantity);
            var totalQuantity = cartItems.Sum(item => item.Quantity);

            // Cập nhật tổng tiền và tổng số lượng vào session
            HttpContext.Session.SetInt32("total", (int)total);
            HttpContext.Session.SetInt32("totalQuantity", totalQuantity);

            return Json(new
            {
                message = "Sản phẩm đã được thêm vào giỏ hàng!",
                cart = cartItems,
                total,
                totalQuantity
            });
        }

        public async Task<IActionResult> SearchProduct(string query)
        {
            var products = await _context.Products
                .Where(p => p.Name.Contains(query) || p.Id.ToString().Contains(query))
                .ToListAsync();

            var output = "";

            if (products.Any())
            {
                foreach (var product in products)
                {
                    output += $@"
                <tr>
                    <td class=""text-center"">{product.Id}</td>
                    <td>{product.Name}</td>
                    <td class=""text-center""><img src='/adminThienAn/image_product/{(product.MainImg ?? "default.png")}' alt='{product.Name}' width='50px'></td>
                    <td><input class='so--luong1 productList-quantity' type='number' value='1' min='1' data-id='{product.Id}'></td>
                    <td class=""text-center"">{product.Price.ToString("N0")}đ</td>
                    <td class='text-center'><a class='btn btn-success btn-sm add-to-cart text-white' data-id='{product.Id}'>Thêm</a></td>
                </tr>";
                }
            }
            else
            {
                output = "<tr><td colspan='6'>Không có sản phẩm nào được tìm thấy.</td></tr>";
            }

            return Content(output);
        }

        private void SaveCartToSession(Dictionary<int, CartDetail> cart)
        {
            var cartJson = JsonConvert.SerializeObject(cart);
            HttpContext.Session.SetString("cart", cartJson);
        }

        // Phương thức xóa sản phẩm khỏi session giỏ hàng
        [HttpPost]
        public IActionResult RemoveProductFromSession(int id)
        {
            // Lấy giỏ hàng từ session
            var cart = HttpContext.Session.GetString("cart");
            var cartItems = string.IsNullOrEmpty(cart)
                ? new List<CartDetail>()
                : JsonConvert.DeserializeObject<List<CartDetail>>(cart);

            // Kiểm tra và xóa sản phẩm khỏi giỏ hàng
            var item = cartItems.FirstOrDefault(i => i.ProductId == id);
            if (item != null)
            {
                cartItems.Remove(item);
                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(cartItems));
            }

            // Tính tổng tiền và tổng số lượng
            var total = cartItems.Sum(item => item.Price * item.Quantity);
            var totalQuantity = cartItems.Sum(item => item.Quantity);
            return Json(new
            {
                cart = cartItems,
                //total = $"{total:n0}₫",
                total = total,
                totalQuantity
            });
        }

        // Phương thức kiểm tra khách hàng
        [HttpPost]
        public async Task<IActionResult> CheckCustomer(string phone)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(u => u.PhoneNumber == phone);

            return Json(new { exists = customer != null });
        }

        // Phương thức thêm khách hàng mới
        [HttpPost]
        public async Task<IActionResult> AddNewCustomer(string fullname, string phone)
        {
            if (await _context.Users.AnyAsync(u => u.PhoneNumber == phone))
                return Json(new { success = false, message = "Số điện thoại đã tồn tại" });

            var customer = new Customer
            {
                FullName = fullname,
                PhoneNumber = phone,
                Username = phone,
                Password = Helpers.PasswordHelper.HashPassword(phone)  // mật khẩu mặc định SĐT
            };

            _context.Users.Add(customer);
            await _context.SaveChangesAsync();

            return Json(new { success = true, customer });
        }

        // Phương thức tạo đơn hàng
        [HttpPost]
        public async Task<IActionResult> CreateOrder(string customerPhone, string address, string paymentMethod, string note)
        {
            var customer = await _context.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == customerPhone);

            if (customer == null)
                return Json(new { error = "Khách hàng không tồn tại" });

            // Lấy giỏ hàng từ session
            var cart = HttpContext.Session.GetString("cart");
            var cartItems = string.IsNullOrEmpty(cart)
                ? new List<CartDetail>()
                : JsonConvert.DeserializeObject<List<CartDetail>>(cart);

            if (!cartItems.Any())
                return Json(new { error = "Giỏ hàng rỗng" });

            var totalQuantity = cartItems.Sum(i => i.Quantity);
            var totalPrice = cartItems.Sum(i => i.Price * i.Quantity);

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Lấy UserId từ ClaimsPrincipal và chuyển đổi thành int?
                var saleStaffIdString = User.Identity.GetUserId();  // Lấy UserId dạng string
                int? saleStaffId = 1;

                // Cố gắng chuyển đổi UserId sang int?, nếu không thành công thì giữ giá trị null
                if (int.TryParse(saleStaffIdString, out int saleStaff))
                {
                    saleStaffId = saleStaff;
                }

                var order = new Order
                {
                    CustomerId = customer.Id,
                    CustomerPhoneNumber = customerPhone,
                    TotalPrice = totalPrice,
                    TotalQuantity = totalQuantity,
                    Address = address,
                    OrderDate = DateTime.Now,
                    Note = note ?? "",
                    PaymentMethod = paymentMethod,
                    PaymentStatus = "Đã thanh toán",
                    InvoiceNumber = "HD" + DateTime.Now.ToString("yyMMddHHmmss"),
                    InvoiceDate = DateTime.Now,
                    SaleStaffId = saleStaffId,  // Cần kiểm tra xem admin đã đăng nhập chưa
                    OrderStatus = (int)Helpers.ConstHelper.OrderStatus.Success
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                foreach (var item in cartItems)
                {
                    var orderDetail = new OrderDetail
                    {
                        OrderId = order.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        PriceAtOrder = item.Price
                    };

                    _context.OrderDetails.Add(orderDetail);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                HttpContext.Session.Remove("cart");
                return Json(new { success = true, orderID = order.Id });
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return Json(new { error = "Đã xảy ra lỗi khi tạo đơn hàng" });
            }
        }
    }
}
