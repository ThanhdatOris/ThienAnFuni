using Microsoft.AspNetCore.Mvc;

namespace ThienAnFuni.Models
{
    public class CheckoutController : Controller
    {
        public IActionResult Index()
        {
            // Kiểm tra xem người dùng đã đăng nhập hay chưa
            if (!User.Identity.IsAuthenticated)
            {
                // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
                return RedirectToAction("Login", "Account");  // Giả sử trang đăng nhập có action Login trong controller Account
            }
            return View();
        }
    }
}
