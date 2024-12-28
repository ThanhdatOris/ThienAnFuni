﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThienAnFuni.Helpers;
using ThienAnFuni.Models;

namespace ThienAnFuni.Controllers
{
    [Authorize(Roles = $"{ConstHelper.RoleManager},{ConstHelper.RoleSaleStaff}")]

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

        #region old Index

        //public async Task<IActionResult> Index()
        //{
        //    ViewData["ActiveMenu"] = "Product";

        //    var products = await _context.Products
        //        .Where(p => p.IsActive)
        //        .Include(p => p.Category)
        //        .Select(p => new
        //        {
        //            p.Id, 
        //            p.Name, 
        //            p.MainImg,
        //            p.IsActive,
        //            p.Price,
        //            CategoryName = p.Category.Name,
        //            QuantityInStock = (_context.Goods
        //                .Where(g => g.ProductId == p.Id)
        //                .Sum(g => (int?)g.Quantity) ?? 0) -
        //                (_context.OrderDetails
        //                .Where(o => o.ProductId == p.Id)
        //                .Sum(o => (int?)o.Quantity) ?? 0)
        //        })
        //        .ToListAsync();

        //    return View(products);
        //}
        #endregion
        public async Task<IActionResult> Index(string categorySlug)
        {
            ViewData["ActiveMenu"] = "Product";

            // Lấy danh sách danh mục (bao gồm danh mục con)
            var categories = await _context.Categories
                .Where(c => c.IsActive && c.ParentId == null)
                .Include(c => c.SubCategories)
                .ToListAsync();

            ViewData["Categories"] = categories;

            // Xử lý duyệt theo danh mục cha lẫn con
            var categorySlugs = new List<string>();
            if (!string.IsNullOrEmpty(categorySlug))
            {
                var categoriesSl = await _context.Categories.Include(c => c.SubCategories).ToListAsync();

                var selectedCategory = categoriesSl.FirstOrDefault(c => c.Slug == categorySlug);
                if (selectedCategory != null)
                {
                    // Thêm category chính vào danh sách
                    categorySlugs.Add(selectedCategory.Slug);

                    // Thêm các subcategory vào danh sách
                    categorySlugs.AddRange(selectedCategory.SubCategories.Select(c => c.Slug));
                }
            }

            // Truy vấn sản phẩm theo các categorySlug
            var productsQuery = _context.Products
                .Where(p => p.IsActive)
                .Include(p => p.Category)  
                .ThenInclude(c => c.SubCategories)  
                .Where(p => string.IsNullOrEmpty(categorySlug) || categorySlugs.Contains(p.Category.Slug));  // Lọc theo categorySlug hoặc subCategorySlug

            var products = await productsQuery
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.MainImg,
                    p.IsActive,
                    p.Price,
                    CategoryName = p.Category.Name,
                    CategorySlug = p.Category.Slug,
                    QuantityInStock = (_context.Goods
                        .Where(g => g.ProductId == p.Id)
                        .Sum(g => (int?)g.Quantity) ?? 0) -
                        (_context.OrderDetails
                        .Where(o => o.ProductId == p.Id)
                        .Sum(o => (int?)o.Quantity) ?? 0)
                })
                .ToListAsync();

            ViewBag.ActiveSlug = categorySlug;

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
        public async Task<IActionResult> Create(Product model, IFormFile MainImage, List<IFormFile> ImageUploadSub)
        {
            ViewData["ActiveMenu"] = "Product";

            // Đặt giá trị mặc định cho sản phẩm
            model.IsImport = false;
            model.IsActive = true;
            model.CreatedDate = DateOnly.FromDateTime(DateTime.Now);

            // Xử lý ảnh chính
            if (MainImage != null && MainImage.Length > 0)
            {
                string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "adminThienAn/image_product");
                Directory.CreateDirectory(uploadDir);

                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(MainImage.FileName)}";
                string filePath = Path.Combine(uploadDir, fileName);

                using var fileStream = new FileStream(filePath, FileMode.Create);
                await MainImage.CopyToAsync(fileStream);

                model.MainImg = fileName;
            }
            else
            {
                model.MainImg = "default.png";
            }

            _context.Products.Add(model);
            await _context.SaveChangesAsync();

            // Xử lý ảnh phụ
            if (ImageUploadSub != null && ImageUploadSub.Count > 0)
            {
                foreach (var image in ImageUploadSub)
                {
                    if (image.Length > 0)
                    {
                        string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "adminThienAn/image_product");
                        Directory.CreateDirectory(uploadDir);

                        string fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                        string filePath = Path.Combine(uploadDir, fileName);

                        using var fileStream = new FileStream(filePath, FileMode.Create);
                        await image.CopyToAsync(fileStream);

                        // Lưu vào ProductImage
                        _context.ProductImages.Add(new ProductImage
                        {
                            ProductId = model.Id,
                            ImgURL = fileName
                        });
                    }
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }


        // GET: AdminProducts/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            ViewData["ActiveMenu"] = "Product";

            var product = await _context.Products
               .Include(p => p.ProductImages)
               .FirstOrDefaultAsync(p => p.Id == id);

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
        public async Task<IActionResult> Edit(int id, Product updatedProduct, IFormFile ImageUpload, List<IFormFile> ImageUploadSub)
        {
            ViewData["ActiveMenu"] = "Product";

            if (id != updatedProduct.Id)
            {
                return BadRequest();
            }

            // Lấy sản phẩm hiện tại
            var product = await _context.Products
                .Include(p => p.ProductImages) // Bao gồm danh sách ảnh phụ
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            // Cập nhật thông tin sản phẩm
            product.Name = updatedProduct.Name;
            product.Price = updatedProduct.Price;
            product.Unit = updatedProduct.Unit;
            product.Material = updatedProduct.Material;
            product.Dimension = updatedProduct.Dimension;
            product.Standard = updatedProduct.Standard;
            product.Color = updatedProduct.Color;
            product.Brand = updatedProduct.Brand;
            product.WarrantyPeriod = updatedProduct.WarrantyPeriod;
            //product.IsActive = updatedProduct.IsActive;
            product.Description = updatedProduct.Description;
            product.CategoryId = updatedProduct.CategoryId;

            // Cập nhật ảnh chính
            if (ImageUpload != null && ImageUpload.Length > 0)
            {
                string uploadDir = Path.Combine("wwwroot/adminThienAn/image_product");
                Directory.CreateDirectory(uploadDir);

                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(ImageUpload.FileName)}";
                string filePath = Path.Combine(uploadDir, fileName);

                // Xóa ảnh cũ (nếu cần)
                if (!string.IsNullOrEmpty(product.MainImg) && product.MainImg != "default.png")
                {
                    var oldPath = Path.Combine(uploadDir, product.MainImg);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                // Lưu ảnh mới
                using var stream = new FileStream(filePath, FileMode.Create);
                await ImageUpload.CopyToAsync(stream);

                product.MainImg = fileName;
            }

            // Xử lý ảnh phụ
            if (ImageUploadSub != null && ImageUploadSub.Count > 0)
            {
                string uploadDir = Path.Combine("wwwroot/adminThienAn/image_product");
                Directory.CreateDirectory(uploadDir);

                foreach (var file in ImageUploadSub)
                {
                    string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    string filePath = Path.Combine(uploadDir, fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(stream);

                    var productImage = new ProductImage
                    {
                        ImgURL = fileName,
                        ProductId = product.Id
                    };

                    _context.ProductImages.Add(productImage);
                }
            }

            _context.Update(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var image = await _context.ProductImages.FindAsync(id);
            if (image == null)
            {
                return NotFound();
            }

            // Xóa file trên server
            var filePath = Path.Combine("wwwroot/adminThienAn/image_product", image.ImgURL);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            // Xóa khỏi database
            _context.ProductImages.Remove(image);
            await _context.SaveChangesAsync();

            return Ok();
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
