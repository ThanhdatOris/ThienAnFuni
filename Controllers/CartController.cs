using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThienAnFuni.Extensions;
using SessionExtensions = ThienAnFuni.Extensions.SessionExtensions;
using ThienAnFuni.Models;

namespace ThienAnFuni.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        private readonly TAF_DbContext _context;

        public CartController(TAF_DbContext context)
        {
            _context = context;
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
            if (product.QuantityInStock < cart.GetValueOrDefault(id)?.Quantity + quantity)
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
                    Product = new Product { Id = product.Id, Name = product.Name, Price = product.Price },
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

    }
}
