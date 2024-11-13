using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
using ThienAnFuni.Data;
using ThienAnFuni.Helpers;
using ThienAnFuni.Models;

namespace ThienAnFuni.Controllers
{
    public class POSController : Controller
    {
        private readonly TAF_DbContext _context;
        private readonly UserManager<User> _userManager;
        public POSController(TAF_DbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
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
            // Lấy thông tin sản phẩm từ cơ sở dữ liệu
            var product = await _context.Products
                .Where(p => p.Id == id)
                .Include(p => p.Category) // Bao gồm thông tin về danh mục nếu cần
                .FirstOrDefaultAsync(p => p.Id == id);

            // Kiểm tra nếu sản phẩm không tồn tại hoặc không có giá
            if (product == null || product.Price == null)
            {
                return Json(new { message = "Sản phẩm không tồn tại hoặc thông tin sản phẩm không đầy đủ!" });
            }

            // Tính số lượng tồn kho của sản phẩm
            var quantityInStock = (_context.Goods
                .Where(g => g.ProductId == id)
                .Sum(g => (int?)g.Quantity) ?? 0) -
                (_context.OrderDetails
                .Where(o => o.ProductId == id)
                .Sum(o => (int?)o.Quantity) ?? 0);

            // Kiểm tra nếu số lượng trong kho không hợp lệ
            if (quantityInStock < 0)
            {
                quantityInStock = 0; // Đảm bảo không có số âm
            }

            // Lấy giỏ hàng từ session
            var cart = HttpContext.Session.GetString("cart");
            var cartItems = string.IsNullOrEmpty(cart) ? new List<CartDetail>() : JsonConvert.DeserializeObject<List<CartDetail>>(cart) ?? new List<CartDetail>();

            // Kiểm tra xem sản phẩm đã có trong giỏ hàng chưa
            var existingProduct = cartItems.FirstOrDefault(item => item.ProductId == id);

            // Nếu sản phẩm đã có trong giỏ hàng
            if (existingProduct != null)
            {
                // Kiểm tra nếu số lượng yêu cầu vượt quá số lượng tồn kho
                if (existingProduct.Quantity + productListQuantity > quantityInStock)
                {
                    return Json(new
                    {
                        message = $"Số lượng yêu cầu vượt quá số lượng tồn kho. Tồn kho hiện tại: {quantityInStock}",
                        total = cartItems.Sum(item => item.Price * item.Quantity),
                        totalQuantity = cartItems.Sum(item => item.Quantity)
                    });
                }

                existingProduct.Quantity += productListQuantity; // Cập nhật số lượng sản phẩm trong giỏ
            }
            else
            {
                // Kiểm tra nếu sản phẩm mới thêm vào giỏ hàng có vượt quá số lượng tồn kho
                if (productListQuantity > quantityInStock)
                {
                    return Json(new
                    {
                        message = $"Số lượng yêu cầu vượt quá số lượng tồn kho. Tồn kho hiện tại: {quantityInStock}",
                        total = cartItems.Sum(item => item.Price * item.Quantity),
                        totalQuantity = cartItems.Sum(item => item.Quantity)
                    });
                }

                // Thêm sản phẩm vào giỏ hàng
                cartItems.Add(new CartDetail
                {
                    ProductId = product.Id,
                    Product = product,
                    Price = product.Price,
                    Quantity = productListQuantity
                });
            }

            // Tính toán tổng tiền và tổng số lượng trong giỏ hàng
            var total = cartItems.Sum(item => item.Price * item.Quantity);
            var totalQuantity = cartItems.Sum(item => item.Quantity);

            // Chuyển giỏ hàng thành danh sách DTO để trả lại cho client
            var cartItemDTOs = cartItems
                .Select(item => new CartItemDTO
                {
                    ProductId = item.ProductId,
                    Name = item.Product?.Name, // Nếu Product null, trả về null
                    MainImg = item.Product?.MainImg, // Nếu Product null, trả về null
                    Price = item.Price,
                    Quantity = item.Quantity
                })
                .ToList();

            // Cấu hình để bỏ qua vòng lặp tham chiếu khi serialize
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            // Lưu lại giỏ hàng trong session dưới dạng JSON
            HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(cartItems, settings));

            // Trả lại thông tin giỏ hàng và tổng tiền
            return Json(new
            {
                message = "Sản phẩm đã được thêm vào giỏ hàng!",
                cart = cartItemDTOs,
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

            // Lấy danh sách các sản phẩm từ cơ sở dữ liệu để thêm lại tên và ảnh sản phẩm
            var productIds = cartItems.Select(c => c.ProductId).ToList();
            var products = _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToList();

            // Tạo danh sách giỏ hàng DTO với thông tin đầy đủ
            var cartItemDTOs = cartItems.Select(item => new CartItemDTO
            {
                ProductId = item.ProductId,
                Name = products.FirstOrDefault(p => p.Id == item.ProductId)?.Name, // Lấy tên sản phẩm
                MainImg = products.FirstOrDefault(p => p.Id == item.ProductId)?.MainImg, // Lấy ảnh sản phẩm
                Price = item.Price,
                Quantity = item.Quantity
            }).ToList();

            // Tính tổng tiền và tổng số lượng
            var total = cartItems.Sum(item => item.Price * item.Quantity);
            var totalQuantity = cartItems.Sum(item => item.Quantity);

            return Json(new
            {
                cart = cartItemDTOs, // Trả lại giỏ hàng với thông tin đầy đủ
                total,
                totalQuantity
            });
        }


        // Phương thức kiểm tra khách hàng
        [HttpPost]
        public async Task<IActionResult> CheckCustomer(string phone)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(u => u.PhoneNumber.Equals(phone));

            return Json(new { exists = customer != null });
        }

        // Phương thức thêm khách hàng mới
        //[HttpPost]
        //public async Task<IActionResult> AddNewCustomer(string fullname, string phone)
        //{
        //    if (await _context.Users.AnyAsync(u => u.PhoneNumber == phone))
        //        return Json(new { success = false, message = "Số điện thoại đã tồn tại" });

        //    var customer = new Customer
        //    {
        //        FullName = fullname,
        //        PhoneNumber = phone,
        //        UserName = phone,
        //        //Password = Helpers.PasswordHelper.HashPassword(phone)  // mật khẩu mặc định SĐT
        //    };
        //    // Tạo tài khoản cho khách hàng với mật khẩu mặc định là số điện thoại
        //    var result = await _userManager.CreateAsync(customer, phone);

        //    // Kiểm tra xem việc tạo tài khoản có thành công không
        //    if (!result.Succeeded)
        //    {
        //        // Nếu không thành công, trả về lỗi
        //        return Json(new { success = false, message = "Lỗi khi tạo tài khoản: " + string.Join(", ", result.Errors.Select(e => e.Description)) });
        //    }

        //    _context.Users.Add(customer);
        //    await _context.SaveChangesAsync();

        //    return Json(new { success = true, customer });
        //}

        [HttpPost]
        public async Task<IActionResult> AddNewCustomer(string fullname, string phone)
        {
            // Kiểm tra xem số điện thoại đã tồn tại trong cơ sở dữ liệu chưa
            if (await _context.Users.AnyAsync(u => u.PhoneNumber == phone))
                return Json(new { success = false, message = "Số điện thoại đã tồn tại" });

            // Khởi tạo đối tượng Customer
            var customer = new Customer
            {
                FullName = fullname,
                PhoneNumber = phone,
                UserName = phone, // Sử dụng số điện thoại làm tên đăng nhập
            };
            // MK: !aK + phone

            // Tạo tài khoản cho khách hàng với mật khẩu mặc định là số điện thoại
            var result = await _userManager.CreateAsync(customer, "!aK" + phone);

            // Kiểm tra xem việc tạo tài khoản có thành công không
            if (!result.Succeeded)
            {
                // Tạo thông báo lỗi với các lỗi từ Identity
                var errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));

                // Trả về lỗi với mã trạng thái 400 và thông điệp lỗi
                return BadRequest(new { success = false, message = "Lỗi khi tạo tài khoản: " + errorMessage });
            }

            // Không cần thêm lại vào _context.Users nếu CreateAsync đã thành công
            await _context.SaveChangesAsync();

            // Trả về thông tin khách hàng nếu thành công
            return Json(new { success = true, customer });
        }

        // Phương thức tạo đơn hàng
        [HttpPost]
        public async Task<IActionResult> CreateOrder(string customerPhone, string address, int paymentMethod, string note)
        {
            //VALIDATE
            if (string.IsNullOrEmpty(customerPhone))
                return Json(new { error = "Số điện thoại khách hàng không được để trống" });

            if (string.IsNullOrEmpty(address))
                return Json(new { error = "Địa chỉ không được để trống" });

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

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Unauthorized("Không tìm thấy người dùng hiện tại.");
                }

                // Lấy danh sách các role của người dùng hiện tại
                var userRoles = await _userManager.GetRolesAsync(user);

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
                    PaymentStatus = (int?)ConstHelper.PaymentStatus.Paid,
                    InvoiceNumber = "HD" + DateTime.Now.ToString("yyMMddHHmmss") + customer.Id,
                    InvoiceDate = DateTime.Now,
                    SaleStaffId = userId,  // Có cần kiểm tra xem admin đã đăng nhập ?
                    ManagerId = userId,
                    OrderStatus = (int)ConstHelper.OrderStatus.Pending

                };


                // Kiểm tra nếu người dùng là SaleStaff
                if (userRoles.Contains(ConstHelper.RoleSaleStaff))
                {
                    order.SaleStaffId = userId;
                    order.ManagerId = null; // Đảm bảo rằng ManagerId không được gán
                }
                // Kiểm tra nếu người dùng là Manager
                else if (userRoles.Contains(ConstHelper.RoleManager))
                {
                    order.ManagerId = userId;
                    order.SaleStaffId = null; // Đảm bảo rằng SaleStaffId không được gán
                }

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
