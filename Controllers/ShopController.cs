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
        #region Old Index
        //public IActionResult Index(string query, string slug, int page = 1, string sortOrder = null, decimal? minPrice = null, decimal? maxPrice = null, string color = null)
        //{
        //    int pageSize = 6;

        //    // Lấy danh mục nếu có slug
        //    Category category = null;
        //    List<int> categoryIds = new List<int>();

        //    if (!string.IsNullOrEmpty(slug))
        //    {
        //        category = _context.Categories.FirstOrDefault(c => c.Slug == slug);
        //        if (category != null)
        //        {
        //            categoryIds = GetAllCategoryIds(category.Id); // Lấy danh mục cha và con
        //        }
        //    }

        //    // Lọc sản phẩm
        //    var productsQuery = _context.Products
        //        .Where(p => p.IsActive == true && p.IsImport == true);

        //    if (!string.IsNullOrEmpty(query))
        //    {
        //        productsQuery = productsQuery.Where(p => p.Name.Contains(query));
        //    }

        //    if (categoryIds.Any())
        //    {
        //        productsQuery = productsQuery.Where(p => p.CategoryId.HasValue && categoryIds.Contains(p.CategoryId.Value));
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

        //    // Lọc theo màu sắc
        //    if (!string.IsNullOrEmpty(color))
        //    {
        //        if (color == "Tự nhiên")
        //        {
        //            // Nếu là màu tự nhiên, lấy cả sản phẩm màu nâu
        //            productsQuery = productsQuery.Where(p => p.Color == "Nâu" || p.Color == "Tự nhiên");
        //        }
        //        else
        //        {
        //            productsQuery = productsQuery.Where(p => p.Color == color);
        //        }
        //    }

        //    // Phân trang
        //    var products = productsQuery.ToPagedList(page, pageSize);

        //    // Truyền thêm thông tin vào View
        //    ViewBag.SortOrder = sortOrder;
        //    ViewBag.Count = products.TotalItemCount;
        //    ViewBag.CategoryName = category?.Name;
        //    ViewBag.ActiveSlug = category?.Slug;
        //    ViewBag.MinPrice = minPrice ?? 1000000; // Giá trị mặc định
        //    ViewBag.MaxPrice = maxPrice ?? 30000000; // Giá trị mặc định
        //    ViewBag.SelectedColor = color; // Lưu màu được chọn để hiển thị lại

        //    return View(products);
        //}
        #endregion
        public IActionResult Index(
         string query,
         string slug,
         int page = 1,
         string sortOrder = null,
         decimal? minPrice = null,
         decimal? maxPrice = null,
         string color = null)
        {
            int pageSize = 9;

            // Lấy danh mục nếu có slug
            Category category = null;
            List<int> categoryIds = new List<int>();

            if (!string.IsNullOrEmpty(slug))
            {
                category = _context.Categories.FirstOrDefault(c => c.Slug == slug);
                if (category != null)
                {
                    categoryIds = GetAllCategoryIds(category.Id); // Get all ID của danh mục cha và con
                }
            }

            // Prepare Products query
            var productsQuery = _context.Products
                .Where(p => p.IsActive && p.IsImport);

            // Lọc theo từ khóa tìm kiếm
            if (!string.IsNullOrEmpty(query))
            {
                productsQuery = productsQuery.Where(p => p.Name.Contains(query));
            }

            // Lọc theo danh mục
            if (categoryIds.Any())
            {
                productsQuery = productsQuery.Where(p => p.CategoryId.HasValue && categoryIds.Contains(p.CategoryId.Value));
            }

            // Lọc theo khoảng giá
            if (minPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price >= (double)minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price <= (double)maxPrice.Value);
            }

            // Lọc theo màu sắc (tối ưu phần lọc màu)
            if (!string.IsNullOrEmpty(color))
            {
                productsQuery = productsQuery.Where(p =>
                    color == "Tự nhiên" ? (p.Color == "Nâu" || p.Color == "Tự nhiên") : p.Color == color
                );
            }

            // Sắp xếp sản phẩm theo keyword syntax: switch expression
            productsQuery = sortOrder switch
            {
                "price_asc" => productsQuery.OrderBy(p => p.Price),
                "price_desc" => productsQuery.OrderByDescending(p => p.Price),
                "name_asc" => productsQuery.OrderBy(p => p.Name),
                "name_desc" => productsQuery.OrderByDescending(p => p.Name),
                _ => productsQuery.OrderBy(p => p.Id), // Sắp xếp mặc định theo Id
            };

            // Phân trang
            var products = productsQuery.ToPagedList(page, pageSize);

            // Truyền dữ liệu vào ViewBag để hiển thị lại thông tin đã lọc
            ViewBag.SortOrder = sortOrder;
            ViewBag.Count = products.TotalItemCount;
            ViewBag.CategoryName = category?.Name;
            ViewBag.ActiveSlug = category?.Slug;
            ViewBag.MinPrice = minPrice ?? 1000000;
            ViewBag.MaxPrice = maxPrice ?? 20000000;
            ViewBag.SelectedColor = color;
            ViewBag.Query = query;

            return View(products);
        }

        public IActionResult Detail(int id)
        {
            var product = _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .FirstOrDefault(p => p.Id == id);

            // Lấy 4 sp liên quan trừ sp hiện tại đang xem
            var relatedProducts = _context.Products
                .Where(p => p.CategoryId == product.CategoryId && p.Id != id)
                .Take(4)
                .ToList();

            if (product == null)
            {
                return RedirectToAction("Index");
            }

            // Tính tổng SL đã bán
            int soldQuantity = _context.OrderDetails
                .Where(od => od.ProductId == id)  // ... của sp này
                .Sum(od => od.Quantity);

            // Lấy tổng SL nhập
            int totalQuantityInStock = _context.Goods
                .Where(g => g.ProductId == id)  // ... của sp này
                .Sum(g => (int?)g.Quantity) ?? 0;

            // Tính SL tồn
            int availableQuantity = totalQuantityInStock - soldQuantity;

            ViewBag.AvailableQuantity = availableQuantity;
            ViewBag.RelatedProducts = relatedProducts;

            return View(product);
        }

        public async Task<IActionResult> ProductsByCategory(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound("Danh mục không hợp lệ.");
            }

            var category = await _context.Categories
                .Include(c => c.Products)
                .Where(c => c.IsActive)
                .FirstOrDefaultAsync(c => c.Slug == slug);

            if (category == null)
            {
                return NotFound("Danh mục không tồn tại.");
            }

            var products = category.Products;

            if (products == null || !products.Any())
            {
                ViewBag.Message = "Không có sản phẩm nào trong danh mục này.";
            }

            return View(products);
        }
    }
}
