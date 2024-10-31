using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ThienAnFuni.Models;

namespace ThienAnFuni.Controllers
{
    public class ShipmentsController : Controller
    {
        private const string ShipmentSessionKey = "Shipment";
        private readonly TAF_DbContext _context;

        public ShipmentsController(TAF_DbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            ViewData["ActiveMenu"] = "Shipment";

            try
            {
                var suppliers = await _context.Suppliers.ToListAsync();

                if (suppliers == null || !suppliers.Any())
                {
                    ViewBag.SupplierErrorMessage = "Không có nhà cung cấp nào trong cơ sở dữ liệu.";
                }

                ViewBag.Suppliers = suppliers;

                return View();
            }
            catch (Exception ex)
            {
                // Ghi log exception và hiển thị lỗi tổng quát
                ViewBag.ErrorMessage = "Đã xảy ra lỗi khi tải dữ liệu: " + ex.Message;
                return View();
            }
        }
        // Tìm kiếm sản phẩm
        [HttpGet]
        //public IActionResult SearchProduct(string keyword)
        //{
        //    var products = _context.Products
        //     .Where(p => p.Name.Contains(keyword) || p.Id.ToString() == keyword)
        //     .Select(p => new
        //     {
        //         p.Id,
        //         p.Name,
        //         //p.MainImg,
        //         MainImg = p.MainImg != null ? Url.Content("~/adminThienAn/image_product/" + p.MainImg) : Url.Content("~/adminThienAn/image_product/default.png"),
        //         CategoryName = p.Category != null ? p.Category.Name : "Không có",
        //         p.Price
        //     })
        //     .ToList();


        //    return Json(products);
        //}
        public IActionResult SearchProduct(string keyword)
        {
            var products = _context.Products
                .Where(p => p.Name.Contains(keyword) || p.Id.ToString() == keyword)
                .Include(p => p.Category) // Bao gồm cả Category để lấy thông tin nếu có
                .ToList();

            // Cập nhật đường dẫn hình ảnh đầy đủ cho từng sản phẩm
            var result = products.Select(p => new
            {
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                p.Unit,
                p.Material,
                p.Dimension,
                p.Standard,
                p.Color,
                p.Type,
                p.Brand,
                p.WarrantyPeriod,
                p.IsActive,
                MainImg = p.MainImg != null ? Url.Content("~/adminThienAn/image_product/" + p.MainImg) : Url.Content("~/adminThienAn/image_product/default.png"),
                CategoryName = p.Category != null ? p.Category.Name : "Không có"
            });

            return Json(result);
        }

        // Thêm sản phẩm vào lô hàng trong session
        [HttpPost]
        public IActionResult AddToShipment(int productId, int quantity, double importPrice)
        {
            // Kiểm tra xem cơ sở dữ liệu có sẵn không
            if (_context.Products == null)
            {
                return StatusCode(500, "Database context is not initialized.");
            }

            // Kiểm tra sản phẩm
            var product = _context.Products.FirstOrDefault(p => p.Id == productId);
            if (product == null)
                return NotFound("Product not found");

            // Lấy lô hàng
            var shipment = GetShipment();
            if (shipment == null)
            {
                return StatusCode(500, "Shipment not found.");
            }

            // Kiểm tra xem Goods có được khởi tạo không
            if (shipment.Goods == null)
            {
                shipment.Goods = new List<Goods>();
            }

            var goodsItem = shipment.Goods.FirstOrDefault(g => g.ProductId == productId);

            if (goodsItem != null)
            {
                // Cập nhật số lượng và tổng giá nếu sản phẩm đã tồn tại
                goodsItem.Quantity += quantity;
                goodsItem.TotalPrice = goodsItem.Quantity * importPrice;
            }
            else
            {
                // Thêm sản phẩm mới vào lô hàng
                shipment.Goods.Add(new Goods
                {
                    ProductId = product.Id,
                    Quantity = quantity,
                    ImportPrice = importPrice,
                    TotalPrice = importPrice * quantity
                });
            }

            SaveShipmentSession(shipment);
            return Json(shipment.Goods);
        }

        [HttpPost]
        public IActionResult RemoveFromShipment(int productId)
        {
            var shipment = GetShipment();
            var goodsItem = shipment.Goods.FirstOrDefault(g => g.ProductId == productId);

            if (goodsItem != null)
            {
                shipment.Goods.Remove(goodsItem);
                SaveShipmentSession(shipment);
                return Json(shipment.Goods);
            }

            return NotFound("Không tìm thấy sản phẩm trong đơn hàng");
        }


        private Shipment GetShipment()
        {
            var session = HttpContext.Session.GetString(ShipmentSessionKey);
            return session != null ? JsonConvert.DeserializeObject<Shipment>(session) : new Shipment();
        }

        [HttpGet]
        public IActionResult GetShipmentGoods()
        {
            var shipment = GetShipment(); // Lấy thông tin shipment từ session
            return Json(shipment.Goods);
        }

        private void SaveShipmentSession(Shipment shipment)
        {
            HttpContext.Session.SetString(ShipmentSessionKey, JsonConvert.SerializeObject(shipment));
        }
    }
}
