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
        //public IActionResult AddToShipment(int productId, int quantity, double importPrice)
        //{
        //    // Kiểm tra xem cơ sở dữ liệu có sẵn không
        //    if (_context.Products == null)
        //    {
        //        return StatusCode(500, "Database context is not initialized.");
        //    }

        //    // Kiểm tra sản phẩm
        //    var product = _context.Products.FirstOrDefault(p => p.Id == productId);
        //    if (product == null)
        //        return NotFound("Product not found");

        //    // Lấy lô hàng
        //    var shipment = GetShipment();
        //    if (shipment == null)
        //    {
        //        return StatusCode(500, "Shipment not found.");
        //    }

        //    // Kiểm tra xem Goods có được khởi tạo không
        //    if (shipment.Goods == null)
        //    {
        //        shipment.Goods = new List<Goods>();
        //    }

        //    var goodsItem = shipment.Goods.FirstOrDefault(g => g.ProductId == productId);

        //    if (goodsItem != null)
        //    {
        //        // Cập nhật số lượng và tổng giá nếu sản phẩm đã tồn tại
        //        goodsItem.Quantity += quantity;
        //        goodsItem.TotalPrice = goodsItem.Quantity * importPrice;
        //    }
        //    else
        //    {
        //        // Thêm sản phẩm mới vào lô hàng
        //        shipment.Goods.Add(new Goods
        //        {
        //            ProductId = product.Id,
        //            Quantity = quantity,
        //            ImportPrice = importPrice,
        //            TotalPrice = importPrice * quantity
        //        });
        //    }

        //    SaveShipmentSession(shipment);
        //    return Json(shipment.Goods);
        //}

        public IActionResult AddToShipment(int productId, int quantity, double importPrice)
        {
            // Kiểm tra cơ sở dữ liệu có sẵn hay không
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

            // Tìm sản phẩm trong lô hàng hiện tại
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
                    Product = product,
                    Quantity = quantity,
                    ImportPrice = importPrice,
                    TotalPrice = importPrice * quantity,
                    ShipmentId = shipment.Id
                });
            }

            // Cập nhật tổng giá và tổng số lượng của lô hàng
            shipment.TotalQuantity = shipment.Goods.Sum(g => g.Quantity);
            shipment.TotalPrice = shipment.Goods.Sum(g => g.TotalPrice);

            SaveShipmentSession(shipment);

            return Json(shipment.Goods.Select(g => new
            {
                g.ProductId,
                ProductName = g?.Product?.Name ?? "N/A",
                ProductImage = $"/adminThienAn/image_product/{g?.Product?.MainImg ?? "default.png"}",
                g.Quantity,
                Category = product.Category?.Name ?? "N/A",
                g.ImportPrice
            }));
        }



        [HttpPost]
        public IActionResult RemoveFromShipment(int productId)
        {
            var shipment = GetShipment();
            var goodsItem = shipment?.Goods?.FirstOrDefault(g => g.ProductId == productId);

            if (goodsItem != null)
            {
                shipment?.Goods?.Remove(goodsItem);
                SaveShipmentSession(shipment);

                // Trả về danh sách hàng hóa đã cập nhật với thông tin đầy đủ về sản phẩm
                return Json(shipment.Goods.Select(g => new
                {
                    g.ProductId,
                    ProductName = g.Product?.Name ?? "N/A", // Lấy tên sản phẩm
                    ProductImage = $"/adminThienAn/image_product/{g.Product?.MainImg ?? "default.png"}", // Lấy hình ảnh sản phẩm
                    g.Quantity,
                    Category = g.Product?.Category?.Name ?? "N/A", // Lấy tên danh mục
                    g.ImportPrice
                }));
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
            var shipment = GetShipment();
            var result = shipment?.Goods?.Select(g => new
            {
                g.ProductId,
                ProductName = g.Product?.Name ?? "Không có tên",
                ProductImage = g.Product?.MainImg != null ? Url.Content("~/adminThienAn/image_product/" + g.Product.MainImg) : Url.Content("~/adminThienAn/image_product/default.png"),
                g.Quantity,
                Category = g.Product?.Category?.Name ?? "Không có",
                g.ImportPrice
            }).ToList();

            return Json(result);
        }


        private void SaveShipmentSession(Shipment shipment)
        {
            HttpContext.Session.SetString(ShipmentSessionKey, JsonConvert.SerializeObject(shipment));
        }

        public IActionResult SaveShipmentToDatabase(DateTime receiptDate, int supplierId, int managerId)
        {
            var shipment = GetShipment();

            // Kiểm tra nếu lô hàng trống
            if (shipment == null || shipment.Goods == null || !shipment.Goods.Any())
            {
                return BadRequest("Lô hàng trống, không thể lưu.");
            }

            // Cập nhật thông tin lô hàng
            shipment.ReceiptDate = receiptDate;
            shipment.SupplierId = supplierId;
            shipment.ManagerId = managerId;

            // Lưu vào cơ sở dữ liệu
            _context.Shipments.Add(shipment);
            _context.SaveChanges();

            // Xóa session sau khi lưu
            HttpContext.Session.Remove(ShipmentSessionKey);

            return Ok("Phiếu nhập đã được tạo thành công.");
        }

    }
}
