using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ThienAnFuni.Models;

public class CheckoutController : Controller
{
    private readonly UserManager<User> _userManager;

    // truy xuất thông tin người dùng hiện tại
    public CheckoutController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> CheckOutSV()
    {
        var user = await _userManager.GetUserAsync(User); // Lấy thông tin người dùng hiện tại
        var phoneNumber = user?.PhoneNumber; // Lấy số điện thoại của người dùng

        // Truyền số điện thoại vào ViewBag hoặc Model để sử dụng trong View
        ViewBag.PhoneNumber = phoneNumber;

        return View();
    }
}
