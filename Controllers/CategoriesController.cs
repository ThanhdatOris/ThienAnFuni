﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ThienAnFuni.Helpers;
using ThienAnFuni.Models;

namespace ThienAnFuni.Controllers
{
    [Authorize(Roles = $"{ConstHelper.RoleManager}")]

    public class CategoriesController : Controller
    {
        private readonly TAF_DbContext _context;

        public CategoriesController(TAF_DbContext context)
        {
            _context = context;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            ViewData["ActiveMenu"] = "Category";

            return View(await _context.Categories.ToListAsync());
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewData["ActiveMenu"] = "Category";

            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["ActiveMenu"] = "Category";

            ViewData["ParentCategories"] = _context.Categories
               .Select(c => new SelectListItem
               {
                   Value = c.Id.ToString(),
                   Text = c.Name
               })
               .ToList();

            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ParentId,Image,Name,IsActive")] Category category, IFormFile Image)
        {
            ViewData["ActiveMenu"] = "Category";

            // Tạo slug từ Name
            category.Slug = category.Name.ToSlug();

            // Kiểm tra trùng slug
            bool isSlugDuplicate = await _context.Categories.AnyAsync(c => c.Slug == category.Slug);
            if (isSlugDuplicate)
            {
                TempData["ErrorMessage"] = "Tên danh mục đã tồn tại. Vui lòng chọn tên khác.";

                // Hiển thị lại form với thông báo lỗi
                ViewData["ParentCategories"] = _context.Categories
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    })
                    .ToList();
                return View(category);
            }

            if (Image != null && Image.Length > 0)
            {
                var fileExtension = Path.GetExtension(Image.FileName);
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/customerThienAn/img/categories");

                // Kiểm tra và tạo thư mục nếu chưa tồn tại
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                var filePath = Path.Combine(uploadFolder, fileName);

                // Đảm bảo tên tệp là duy nhất
                while (System.IO.File.Exists(filePath))
                {
                    fileName = $"{Guid.NewGuid()}{fileExtension}";
                    filePath = Path.Combine(uploadFolder, fileName);
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Image.CopyToAsync(stream);
                }

                category.Image = fileName;
            }

            // Nếu không trùng thì thêm vào cơ sở dữ liệu
            _context.Add(category);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Tạo danh mục thành công!";
            return RedirectToAction(nameof(Index));
        }


        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["ActiveMenu"] = "Category";

            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            // Load danh sách danh mục cha để hiển thị trong dropdown
            ViewData["ParentCategories"] = _context.Categories
                .Where(c => c.Id != id)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToList();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ParentId,Name,IsActive,Slug,Image")] Category category, IFormFile Image)
        {
            ViewData["ActiveMenu"] = "Category";

            if (id != category.Id)
            {
                return NotFound();
            }

            try
            {
                category.Slug = category.Name.ToSlug();

                if (Image != null && Image.Length > 0)
                {
                    var fileExtension = Path.GetExtension(Image.FileName);
                    var fileName = $"{Guid.NewGuid()}{fileExtension}";
                    var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/customerThienAn/img/categories");

                    // Kiểm tra và tạo thư mục nếu chưa tồn tại
                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }

                    var filePath = Path.Combine(uploadFolder, fileName);

                    // Đảm bảo tên tệp là duy nhất
                    while (System.IO.File.Exists(filePath))
                    {
                        fileName = $"{Guid.NewGuid()}{fileExtension}";
                        filePath = Path.Combine(uploadFolder, fileName);
                    }

                    // Xóa hình ảnh cũ nếu có và chỉ khi người dùng tải lên hình ảnh mới
                    if (!string.IsNullOrEmpty(category.Image))
                    {
                        var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/customerThienAn/img/categories", category.Image);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            try
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                            catch (Exception ex)
                            {
                                // Log lỗi nếu cần thiết
                                Console.WriteLine($"Error deleting file: {ex.Message}");
                            }
                        }
                    }

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await Image.CopyToAsync(stream);
                    }

                    category.Image = fileName;
                }
                else
                {
                    // Giữ lại đường dẫn hình ảnh cũ nếu không có hình ảnh mới được tải lên
                    _context.Entry(category).Property(x => x.Image).IsModified = false;
                }

                _context.Update(category);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(category.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            ViewData["ParentCategories"] = _context.Categories
                .Where(c => c.Id != id)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToList();
            return RedirectToAction(nameof(Index));
        }


        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ViewData["ActiveMenu"] = "Category";

            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ViewData["ActiveMenu"] = "Category";

            var category = await _context.Categories
                                  .Include(c => c.Products)
                                  .FirstOrDefaultAsync(c => c.Id == id);

            bool isParentCate = _context.Categories.Any(c => c.ParentId == id);

            if (category == null)
            {
                return NotFound();
            }

            if (isParentCate)
            {
                TempData["ErrorMessage"] = "Phải hủy liên kết với các danh mục con trước!";
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra xem có sản phẩm nào liên kết với danh mục này
            if (category.Products.Any())
            {
                TempData["ErrorMessage"] = "Không thể xóa danh mục này vì vẫn còn sản phẩm liên kết!";
                return RedirectToAction(nameof(Index));
            }

            if (category != null)
            {
                // Xóa hình ảnh khỏi thư mục
                if (!string.IsNullOrEmpty(category.Image))
                {
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/customerThienAn/img/categories", category.Image);
                    if (System.IO.File.Exists(imagePath))
                    {
                        try
                        {
                            System.IO.File.Delete(imagePath);
                        }
                        catch (Exception ex)
                        {
                            // Log lỗi nếu cần thiết
                            Console.WriteLine($"Error deleting file: {ex.Message}");
                        }
                    }
                }

                _context.Categories.Remove(category);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            ViewData["ActiveMenu"] = "Category";

            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
