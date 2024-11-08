using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ThienAnFuni.Data;
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



        // V3 FINAL

        //public async Task<IActionResult> AddProductSession(int id, int productListQuantity = 1)
        //{
        //    // Lấy thông tin sản phẩm từ cơ sở dữ liệu
        //    var product = await _context.Products
        //        .Where(p => p.Id == id)
        //        .Include(p => p.Category) // Bao gồm thông tin về danh mục nếu cần
        //        .FirstOrDefaultAsync(p => p.Id == id);

        //    // Kiểm tra nếu sản phẩm không tồn tại hoặc không có giá
        //    if (product == null || product.Price == null)
        //    {
        //        return Json(new { message = "Sản phẩm không tồn tại hoặc thông tin sản phẩm không đầy đủ!" });
        //    }

        //    // Tính số lượng tồn kho của sản phẩm
        //    var quantityInStock = (_context.Goods
        //        .Where(g => g.ProductId == id)
        //        .Sum(g => (int?)g.Quantity) ?? 0) -
        //        (_context.OrderDetails
        //        .Where(o => o.ProductId == id)
        //        .Sum(o => (int?)o.Quantity) ?? 0);

        //    // Kiểm tra nếu số lượng trong kho không hợp lệ
        //    if (quantityInStock < 0)
        //    {
        //        quantityInStock = 0; // Đảm bảo không có số âm
        //    }

        //    // Lấy giỏ hàng từ session
        //    var cart = HttpContext.Session.GetString("cart");
        //    var cartItems = string.IsNullOrEmpty(cart) ? new List<CartDetail>() : JsonConvert.DeserializeObject<List<CartDetail>>(cart) ?? new List<CartDetail>();

        //    // Kiểm tra xem sản phẩm đã có trong giỏ hàng chưa
        //    var existingProduct = cartItems.FirstOrDefault(item => item.ProductId == id);

        //    // Nếu sản phẩm đã có trong giỏ hàng
        //    if (existingProduct != null)
        //    {
        //        if (existingProduct.Quantity + productListQuantity > quantityInStock)
        //        {
        //            return Json(new { message = $"Số lượng yêu cầu vượt quá số lượng tồn kho. Tồn kho hiện tại: {quantityInStock}" });
        //        }
        //        existingProduct.Quantity += productListQuantity; // Cập nhật số lượng
        //    }
        //    else
        //    {
        //        // Nếu sản phẩm chưa có trong giỏ hàng
        //        if (productListQuantity > quantityInStock)
        //        {
        //            return Json(new { message = $"Số lượng yêu cầu vượt quá số lượng tồn kho. Tồn kho hiện tại: {quantityInStock}" });
        //        }
        //        // Thêm sản phẩm vào giỏ hàng
        //        cartItems.Add(new CartDetail
        //        {
        //            ProductId = product.Id,
        //            Product = product, // Lưu toàn bộ đối tượng sản phẩm
        //            Price = product.Price, // Đảm bảo Price có giá trị
        //            Quantity = productListQuantity,
        //        });
        //    }

        //    // Tính toán tổng tiền và tổng số lượng trong giỏ hàng
        //    var total = cartItems.Sum(item => item.Price * item.Quantity);
        //    var totalQuantity = cartItems.Sum(item => item.Quantity);

        //    // Chuyển dữ liệu giỏ hàng thành DTO để trả lại cho view
        //    var cartItemDTOs = cartItems
        //        .Select(item => new CartItemDTO
        //        {
        //            ProductId = item.ProductId,
        //            Name = item.Product?.Name, // Nếu Product null, trả về null
        //            MainImg = item.Product?.MainImg, // Nếu Product null, trả về null
        //            Price = item.Price,
        //            Quantity = item.Quantity
        //        })
        //        .ToList();

        //    // Cấu hình để bỏ qua vòng lặp tham chiếu khi serialize
        //    JsonSerializerSettings settings = new JsonSerializerSettings
        //    {
        //        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        //    };

        //    // Lưu lại giỏ hàng trong session dưới dạng DTO để tránh vòng lặp khi serialize
        //    HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(cartItems, settings));

        //    // Trả lại thông tin giỏ hàng
        //    return Json(new
        //    {
        //        message = "Sản phẩm đã được thêm vào giỏ hàng!",
        //        cart = cartItemDTOs,
        //        total,
        //        totalQuantity
        //    });
        //}

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
        //public IActionResult RemoveProductFromSession(int id)
        //{
        //    // Lấy giỏ hàng từ session
        //    var cart = HttpContext.Session.GetString("cart");
        //    var cartItems = string.IsNullOrEmpty(cart)
        //        ? new List<CartDetail>()
        //        : JsonConvert.DeserializeObject<List<CartDetail>>(cart);

        //    // Kiểm tra và xóa sản phẩm khỏi giỏ hàng
        //    var item = cartItems.FirstOrDefault(i => i.ProductId == id);
        //    if (item != null)
        //    {
        //        cartItems.Remove(item);
        //        HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(cartItems));
        //    }

        //    // Tính tổng tiền và tổng số lượng
        //    var total = cartItems.Sum(item => item.Price * item.Quantity);
        //    var totalQuantity = cartItems.Sum(item => item.Quantity);
        //    return Json(new
        //    {
        //        cart = cartItems,
        //        //total = $"{total:n0}₫",
        //        total = total,
        //        totalQuantity
        //    });
        //}

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
