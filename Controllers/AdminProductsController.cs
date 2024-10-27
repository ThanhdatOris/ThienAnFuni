using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;
using ThienAnFuni.Models;

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
        var products = await _context.Products
            .Include(p => p.Category)
            .ToListAsync();
        return View(products);
    }

    public async Task<IActionResult> Create()
    {
        var caterories = await _context.Categories.ToListAsync();
        return View(caterories);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Product model, IFormFile ImageUpload)
    {
        if (ModelState.IsValid)
        {
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

            // Thêm sản phẩm vào database
            _context.Products.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        return View(model);
    }
}
