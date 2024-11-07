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

        // Lấy danh sách các lô hàng
        [HttpGet]
        public async Task<IActionResult> ListShipment()
        {
            ViewData["ActiveMenu"] = "Shipment";
            try
            {
                var shipments = await _context.Shipments
                    .Include(s => s.Supplier)
                    .Include(s => s.Manager)
                    .Include(s => s.Goods)
                    .ThenInclude(g => g.Product) // Đảm bảo thông tin Product được lấy
                    .ToListAsync();

                return View(shipments); // Trả về View với dữ liệu shipments
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Đã xảy ra lỗi khi tải dữ liệu: " + ex.Message);
            }
        }
        // Xem chi tiết lô hàng
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            ViewData["ActiveMenu"] = "Shipment";
            try
            {
                var shipment = await _context.Shipments
                    .Include(s => s.Supplier)
                    .Include(s => s.Manager)
                    .Include(s => s.Goods)
                        .ThenInclude(g => g.Product)
                        .ThenInclude(p => p.Category)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (shipment == null)
                {
                    return NotFound("Không tìm thấy lô hàng");
                }

                return View(shipment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Đã xảy ra lỗi khi tải dữ liệu: " + ex.Message);
            }
        }


        // Tìm kiếm sản phẩm
        [HttpGet]
        public IActionResult SearchProduct(string keyword)
        {
            var products = _context.Products
                .Where(p => p.IsActive == true && p.Category.IsActive == true && (p.Name.Contains(keyword) || p.Id.ToString() == keyword))
                .Include(p => p.Category)
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

            if (goodsItem != null && goodsItem.Quantity < quantity)
            {
                return BadRequest("Số lượng nhập vào lớn hơn số lượng trong kho");
            }

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
                    Product = product,
                    ProductId = product.Id,
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
                g.ImportPrice,
                g.TotalPrice
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

        public async Task<IActionResult> SaveShipmentToDatabase(DateTime receiptDate, int supplierId)
        {
            // Lấy thông tin lô hàng từ session
            var sessionShipment = GetShipment();

            // Kiểm tra nếu lô hàng trống
            if (sessionShipment == null || sessionShipment.Goods == null || !sessionShipment.Goods.Any())
            {
                return BadRequest("Lô hàng trống, không thể lưu.");
            }

            // **************BUG**************
            // `managerId` có thể lấy từ session người dùng đang đăng nhập
            int managerId = HttpContext.Session.GetInt32("ManagerId") ?? 1;

            //if (managerId == 0)
            //{
            //    return BadRequest("Không thể xác định người quản lý.");
            //}

            // Tạo đối tượng Shipment mới để lưu vào database
            var shipment = new Shipment
            {
                ReceiptDate = receiptDate,
                SupplierId = supplierId,
                ManagerId = managerId,
                TotalPrice = sessionShipment.Goods.Sum(g => g.TotalPrice),
                TotalQuantity = sessionShipment.Goods.Sum(g => g.Quantity),
                Goods = new List<Goods>(),
                Note = sessionShipment.Note // Chưa thêm trường ghi chú trong session
            };

            // Thêm từng Goods vào Shipment
            foreach (var goodsItem in sessionShipment.Goods)
            {
                var goods = new Goods
                {
                    ProductId = goodsItem.ProductId,
                    Quantity = goodsItem.Quantity,
                    ImportPrice = goodsItem.ImportPrice,
                    TotalPrice = goodsItem.TotalPrice
                };

                // Truy xuất sản phẩm từ bảng Product
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == goodsItem.ProductId);

                if (product != null)
                {
                    // Cập nhật IsImport thành true
                    product.IsImport = true;
                }

                shipment.Goods.Add(goods);
            }

            // Lưu Shipment và các Goods vào database
            await _context.Shipments.AddAsync(shipment);
            await _context.SaveChangesAsync();

            // Xóa session sau khi lưu
            HttpContext.Session.Remove(ShipmentSessionKey);

            return RedirectToAction("listShipment");

        }


        // Helper methods session
        private Shipment? GetShipment()
        {
            var session = HttpContext.Session.GetString(ShipmentSessionKey);
            return session != null ? JsonConvert.DeserializeObject<Shipment>(session) : new Shipment();
        }
        private void SaveShipmentSession(Shipment shipment)
        {
            HttpContext.Session.SetString(ShipmentSessionKey, JsonConvert.SerializeObject(shipment));
        }

    }
}
