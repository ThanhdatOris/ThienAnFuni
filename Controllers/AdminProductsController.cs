using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThienAnFuni.Models;

namespace ThienAnFuni.Controllers
{
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
                .Include(p => p.Category)
                .Where(p => p.IsActive == true)
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

            //if (ModelState.IsValid)
            //{
            if (ImageUpload != null && ImageUpload.Length > 0)
            {
                // Đường dẫn thư mục lưu ảnh
                string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "adminThienAn/image_product");

                // Tạo thư mục nếu chưa tồn tại
                if (!Directory.Exists(uploadDir))
                {
                    Directory.CreateDirectory(uploadDir);
                }

                // Tạo tên file duy nhất cho ảnh
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageUpload.FileName);

                // Đường dẫn đầy đủ đến file
                string filePath = Path.Combine(uploadDir, fileName);

                // Upload file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageUpload.CopyToAsync(fileStream);
                }

                // Lưu tên file vào thuộc tính MainImg
                model.MainImg = fileName;
            }
            else
            {
                // Nếu không có ảnh tải lên, dùng ảnh mặc định
                model.MainImg = "default.png";
            }

            // Thêm sản phẩm vào database
            _context.Products.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
            //}

            return View(model);
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

            if (ModelState.IsValid)
            {
                var product = await _context.Products.FindAsync(id);

                if (product == null)
                {
                    return NotFound();
                }

                // Cập nhật các thông tin được phép
                product.Name = updatedProduct.Name;
                product.Price = updatedProduct.Price;
                product.Unit = updatedProduct.Unit;
                product.Material = updatedProduct.Material;
                product.Dimension = updatedProduct.Dimension;
                product.Standard = updatedProduct.Standard;
                product.Color = updatedProduct.Color;
                product.Type = updatedProduct.Type;
                product.Brand = updatedProduct.Brand;
                product.WarrantyPeriod = updatedProduct.WarrantyPeriod;
                product.IsActive = updatedProduct.IsActive;
                product.Description = updatedProduct.Description;
                product.CategoryId = updatedProduct.CategoryId;

                // Xử lý upload ảnh nếu có
                if (ImageUpload != null && ImageUpload.Length > 0)
                {
                    var fileName = Path.GetFileName(ImageUpload.FileName);
                    var filePath = Path.Combine("wwwroot/adminThienAn/image_product", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageUpload.CopyToAsync(stream);
                    }

                    // Lưu tên ảnh vào product
                    product.MainImg = fileName;
                }

                _context.Update(product);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // Nếu có lỗi, đưa danh mục vào ViewBag lại để hiển thị trong form
            ViewBag.Categories = _context.Categories.ToList();
            return View(updatedProduct);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            // Tìm sản phẩm theo ID
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Cập nhật thuộc tính IsActive thành false
            product.IsActive = false;
            _context.Products.Update(product);

            await _context.SaveChangesAsync();

            // Điều hướng về trang Index
            return RedirectToAction("Index");
        }


    }
}
