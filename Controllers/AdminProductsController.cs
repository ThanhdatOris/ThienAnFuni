using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThienAnFuni.Models;

namespace ThienAnFuni.Controllers
{
    //[Authorize(Roles = "Manager")]
    public class AdminProductsController : Controller
    {
        private readonly TAF_DbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        // Constructor duy nhất cho controller
        public AdminProductsController(TAF_DbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["ActiveMenu"] = "Product";

            var products = await _context.Products
                .Where(p => p.IsActive)
                .Include(p => p.Category)
                .Select(p => new
                {
                    p.Id, // Lưu ID của sản phẩm
                    p.Name, // Lưu tên sản phẩm
                    p.MainImg,
                    p.IsActive,
                    p.Price,
                    CategoryName = p.Category.Name, // Lưu tên danh mục
                    QuantityInStock = (_context.Goods
                        .Where(g => g.ProductId == p.Id)
                        .Sum(g => (int?)g.Quantity) ?? 0) -
                        (_context.OrderDetails
                        .Where(o => o.ProductId == p.Id)
                        .Sum(o => (int?)o.Quantity) ?? 0)
                })
                .ToListAsync();

            return View(products);
        }

        public async Task<IActionResult> ListDeleted()
        {
            ViewData["ActiveMenu"] = "Product";
            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive == false)
                .ToListAsync();
            return View(products);
        }

        [HttpPost]
        public IActionResult DeleteImage(string imageName)
        {
            try
            {
                // Kiểm tra tên file hợp lệ
                if (string.IsNullOrEmpty(imageName))
                {
                    return Json(new { success = false, message = "Tên file không hợp lệ." });
                }

                // Xác định đường dẫn tuyệt đối của file trong thư mục wwwroot
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "adminThienAn/image_product", imageName);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "File không tồn tại." });
                }
            }
            catch
            {
                return Json(new { success = false, message = "Xóa ảnh thất bại." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewData["ActiveMenu"] = "Product";

            try
            {
                // Lấy danh sách danh mục và nhà cung cấp từ cơ sở dữ liệu
                var categories = await _context.Categories.ToListAsync();
                var suppliers = await _context.Suppliers.ToListAsync();

                // Kiểm tra danh mục
                if (categories == null || !categories.Any())
                {
                    ViewBag.CategoryErrorMessage = "Không có danh mục nào trong cơ sở dữ liệu.";
                }

                // Kiểm tra nhà cung cấp
                if (suppliers == null || !suppliers.Any())
                {
                    ViewBag.SupplierErrorMessage = "Không có nhà cung cấp nào trong cơ sở dữ liệu.";
                }

                // Truyền danh sách danh mục và nhà cung cấp
                ViewBag.Categories = categories;
                ViewBag.Suppliers = suppliers;

                // Khởi tạo một đối tượng Product mới
                return View(new Product());
            }
            catch (Exception ex)
            {
                // Ghi log exception và hiển thị lỗi tổng quát
                ViewBag.ErrorMessage = "Đã xảy ra lỗi khi tải dữ liệu: " + ex.Message;
                return View(new Product());
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product model, IFormFile ImageUpload)
        {
            ViewData["ActiveMenu"] = "Product";

            // Đặt giá trị mặc định cho các thuộc tính của sản phẩm mới
            model.IsImport = false;
            model.IsActive = true;
            model.CreatedDate = DateOnly.FromDateTime(DateTime.Now);
            // Kiểm tra và xử lý upload ảnh
            if (ImageUpload != null && ImageUpload.Length > 0)
            {
                string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "adminThienAn/image_product");

                // Tạo thư mục nếu chưa tồn tại
                Directory.CreateDirectory(uploadDir);

                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(ImageUpload.FileName)}";
                string filePath = Path.Combine(uploadDir, fileName);

                // Upload file
                using var fileStream = new FileStream(filePath, FileMode.Create);
                await ImageUpload.CopyToAsync(fileStream);

                model.MainImg = fileName;
            }
            else
            {
                model.MainImg = "default.png"; // Ảnh mặc định nếu không upload ảnh
            }

            _context.Products.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        // GET: AdminProducts/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            ViewData["ActiveMenu"] = "Product";

            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            // Đưa danh mục vào ViewBag để hiện trong select dropdown
            ViewBag.Categories = _context.Categories.ToList();

            return View(product);
        }

        // POST: AdminProducts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product updatedProduct, IFormFile ImageUpload)
        {
            ViewData["ActiveMenu"] = "Product";

            if (id != updatedProduct.Id)
            {
                return BadRequest();
            }

            // Lấy sản phẩm hiện tại từ cơ sở dữ liệu
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Cập nhật các thông tin khác của sản phẩm
            product.Name = updatedProduct.Name;
            product.Price = updatedProduct.Price;
            product.Unit = updatedProduct.Unit;
            product.Material = updatedProduct.Material;
            product.Dimension = updatedProduct.Dimension;
            product.Standard = updatedProduct.Standard;
            product.Color = updatedProduct.Color;
            product.Brand = updatedProduct.Brand;
            product.WarrantyPeriod = updatedProduct.WarrantyPeriod;
            product.IsActive = updatedProduct.IsActive;
            product.Description = updatedProduct.Description;
            product.CategoryId = updatedProduct.CategoryId;

            // Xử lý upload ảnh nếu có ảnh mới
            if (ImageUpload != null && ImageUpload.Length > 0)
            {
                var fileName = Path.GetFileName(ImageUpload.FileName);

                // Xóa hình ảnh cũ nếu có và không phải là "default.png"
                if (!string.IsNullOrEmpty(product.MainImg) && product.MainImg != "default.png")
                {
                    var oldImagePath = Path.Combine("wwwroot/adminThienAn/image_product", product.MainImg);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // Lưu hình ảnh mới
                var filePath = Path.Combine("wwwroot/adminThienAn/image_product", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageUpload.CopyToAsync(stream);
                }

                // Cập nhật tên hình ảnh mới vào product
                product.MainImg = fileName;
            }
            else if (string.IsNullOrEmpty(product.MainImg))
            {
                // Nếu không có hình ảnh tải lên và sản phẩm chưa có hình ảnh, đặt thành "default.png"
                product.MainImg = "default.png";
            }

            _context.Update(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Xóa ảnh nếu cần
            //if (!string.IsNullOrEmpty(product.MainImg) && product.MainImg != "default.png")
            //{
            //    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "adminThienAn/image_product", product.MainImg);
            //    if (System.IO.File.Exists(imagePath))
            //    {
            //        System.IO.File.Delete(imagePath);
            //    }
            //}

            // Cập nhật thuộc tính IsActive thành false
            product.IsActive = false;
            _context.Products.Update(product);

            await _context.SaveChangesAsync();

            // Điều hướng về trang Index
            return RedirectToAction("Index");
        }



    }
}
