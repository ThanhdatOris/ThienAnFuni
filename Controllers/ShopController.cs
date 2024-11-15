using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        //public IActionResult Index(int? page)
        //{
        //    int pageSize = 2; // Số sản phẩm trên mỗi trang
        //    int pageNumber = page ?? 1;

        //    var products = _context.Products
        //        .Where(p => p.IsActive == true && p.IsImport == true)
        //        .OrderBy(p => p.Name) // Thêm sắp xếp nếu cần
        //        .ToPagedList(pageNumber, pageSize);

        //    var count = _context.Products
        //        .Where(p => p.IsActive == true && p.IsImport == true)
        //        .Count();

        //    ViewBag.Count = count;

        //    return View(products);
        //}
        public IActionResult Index(string query, int page = 1)
        {
            int pageSize = 2;

            // Lấy danh sách sản phẩm và lọc theo từ khóa nếu có
            var products = _context.Products
                .Where(p => p.IsActive == true && p.IsImport == true &&
                            (string.IsNullOrEmpty(query) || p.Name.Contains(query)))
                .OrderBy(p => p.Name)
                .ToPagedList(page, pageSize);

            // Đếm số lượng sản phẩm phù hợp
            ViewBag.Count = products.TotalItemCount;

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

    }
}
