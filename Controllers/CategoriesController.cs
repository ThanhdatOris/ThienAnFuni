using System;
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
        public IActionResult Create()
        {
            ViewData["ActiveMenu"] = "Category";

            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ParentId,Name,IsActive")] Category category)
        {
            ViewData["ActiveMenu"] = "Category";

            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
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
                .Where(c => c.Id != id) // Exclude current category to prevent self-reference
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,ParentId,Name,IsActive")] Category category)
        {
            ViewData["ActiveMenu"] = "Category";

            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
                return RedirectToAction(nameof(Index));
            }

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
            if (category == null)
            {
                return NotFound();
            }

            // Kiểm tra xem có sản phẩm nào liên kết với danh mục này
            if (category.Products.Any())  // Nếu có sản phẩm liên quan
            {
                // Thông báo lỗi cho người dùng
                TempData["ErrorMessage"] = "Không thể xóa danh mục này vì vẫn còn sản phẩm liên kết!";
                return RedirectToAction(nameof(Index));  // Quay lại trang danh sách
            }

            if (category != null)
            {
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
