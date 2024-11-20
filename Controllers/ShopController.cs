using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using ThienAnFuni.Models;
using X.PagedList.Extensions;

namespace ThienAnFuni.Controllers
{
    public class ShopController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TAF_DbContext _context;

        public ShopController(ILogger<HomeController> logger, TAF_DbContext context)
        {
            _logger = logger;
            _context = context;
        }


        //public IActionResult Index(string query, string slug, int page = 1, string sortOrder = null, decimal? minPrice = null, decimal? maxPrice = null)
        //{
        //    int pageSize = 2;

        //    // Lấy danh mục nếu có slug
        //    Category category = null;
        //    if (!string.IsNullOrEmpty(slug))
        //    {
        //        category = _context.Categories.FirstOrDefault(c => c.Slug == slug);
        //    }

        //    // Lọc sản phẩm
        //    var productsQuery = _context.Products
        //        .Where(p => p.IsActive == true && p.IsImport == true);

        //    if (!string.IsNullOrEmpty(query))
        //    {
        //        productsQuery = productsQuery.Where(p => p.Name.Contains(query));
        //    }

        //    if (category != null)
        //    {
        //        productsQuery = productsQuery.Where(p => p.CategoryId == category.Id);
        //    }

        //    // Lọc giá
        //    if (minPrice.HasValue)
        //    {
        //        productsQuery = productsQuery.Where(p => p.Price >= (double)minPrice.Value);
        //    }

        //    if (maxPrice.HasValue)
        //    {
        //        productsQuery = productsQuery.Where(p => p.Price <= (double)maxPrice.Value);
        //    }

        //    // Phân trang
        //    var products = productsQuery.ToPagedList(page, pageSize);

        //    // Truyền thêm thông tin vào View
        //    ViewBag.SortOrder = sortOrder;
        //    ViewBag.Count = products.TotalItemCount;
        //    ViewBag.CategoryName = category?.Name;
        //    ViewBag.MinPrice = minPrice ?? 1000000; // Giá trị mặc định
        //    ViewBag.MaxPrice = maxPrice ?? 30000000; // Giá trị mặc định

        //    return View(products);
        //}

        #region Helper methods
        private List<int> GetAllCategoryIds(int categoryId)
        {
            var categoryIds = new List<int> { categoryId };

            // Lấy các danh mục con trực tiếp
            var subCategories = _context.Categories
                .Where(c => c.ParentId == categoryId)
                .ToList();

            // Đệ quy lấy ID của danh mục con
            foreach (var subCategory in subCategories)
            {
                categoryIds.AddRange(GetAllCategoryIds(subCategory.Id));
            }

            return categoryIds;
        }
        #endregion

        public IActionResult Index(string query, string slug, int page = 1, string sortOrder = null, decimal? minPrice = null, decimal? maxPrice = null, string color = null)
        {
            int pageSize = 2;

            // Lấy danh mục nếu có slug
            Category category = null;
            List<int> categoryIds = new List<int>();

            if (!string.IsNullOrEmpty(slug))
            {
                category = _context.Categories.FirstOrDefault(c => c.Slug == slug);
                if (category != null)
                {
                    categoryIds = GetAllCategoryIds(category.Id); // Lấy danh mục cha và con
                }
            }

            // Lọc sản phẩm
            var productsQuery = _context.Products
                .Where(p => p.IsActive == true && p.IsImport == true);

            if (!string.IsNullOrEmpty(query))
            {
                productsQuery = productsQuery.Where(p => p.Name.Contains(query));
            }

            if (categoryIds.Any())
            {
                productsQuery = productsQuery.Where(p => p.CategoryId.HasValue && categoryIds.Contains(p.CategoryId.Value));
            }
             
            // Lọc giá
            if (minPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price >= (double)minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price <= (double)maxPrice.Value);
            }

            // Lọc theo màu sắc
            if (!string.IsNullOrEmpty(color))
            {
                if (color == "Tự nhiên")
                {
                    // Nếu là màu tự nhiên, lấy cả sản phẩm màu nâu
                    productsQuery = productsQuery.Where(p => p.Color == "Nâu" || p.Color == "Tự nhiên");
                }
                else
                {
                    productsQuery = productsQuery.Where(p => p.Color == color);
                }
            }

            // Phân trang
            var products = productsQuery.ToPagedList(page, pageSize);

            // Truyền thêm thông tin vào View
            ViewBag.SortOrder = sortOrder;
            ViewBag.Count = products.TotalItemCount;
            ViewBag.CategoryName = category?.Name;
            ViewBag.MinPrice = minPrice ?? 1000000; // Giá trị mặc định
            ViewBag.MaxPrice = maxPrice ?? 30000000; // Giá trị mặc định
            ViewBag.SelectedColor = color; // Lưu màu được chọn để hiển thị lại

            return View(products);
        }

        public IActionResult Detail(int id)
        {
            var product = _context.Products
                .Include(p => p.Category)
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                RedirectToAction("Index");
            }

            return View(product);
        }

        public async Task<IActionResult> ProductsByCategory(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound("Danh mục không hợp lệ.");
            }

            // Tìm danh mục theo slug
            var category = await _context.Categories
                .Include(c => c.Products)
                .Where(c => c.IsActive)
                .FirstOrDefaultAsync(c => c.Slug == slug);

            if (category == null)
            {
                return NotFound("Danh mục không tồn tại.");
            }

            // Lấy danh sách sản phẩm
            var products = category.Products;

            return View(products);
        }


    }
}
