using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThienAnFuni.Models;
using ThienAnFuni.ViewModels;
using ThienAnFuni.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
namespace ThienAnFuni.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            // Kiểm tra xem người dùng đã đăng nhập chưa
            if (!User.Identity.IsAuthenticated)
            // Chưa đăng nhập => đk là T => thực hiện if
            // Đăng nhập => đk là F => bỏ qua if
            {
                // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập và lưu lại trang người dùng muốn vào (nếu có)
                return RedirectToAction("Login", "Account", new { ReturnUrl = "/Checkout/Index" });
            }
            return View();
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem số điện thoại đã tồn tại trong hệ thống chưa
                if (await _userManager.Users.AnyAsync(u => u.PhoneNumber == model.PhoneNumber))
                {
                    ModelState.AddModelError("", "Số điện thoại đã được sử dụng.");
                    return View(model);
                }

                var customer = new Customer
                {
                    UserName = model.Username,
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                    Gender = model.Gender,
                    DateOfBirth = model.DateOfBirth,
                    IsActive = true
                };

                var result = await _userManager.CreateAsync(customer, model.Password);
                if (result.Succeeded)
                {
                    // Gán vai trò "Customer" cho người dùng
                    await _userManager.AddToRoleAsync(customer, "Customer");

                    // Đăng nhập ngay sau khi tạo tài khoản
                    await _signInManager.SignInAsync(customer, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                // Nếu tạo tài khoản không thành công, hiển thị lỗi
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            // Lưu lại URL người dùng muốn truy cập sau khi đăng nhập
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(model.Username);

                    // Kiểm tra nếu user không null và đảm bảo không có giá trị null cho các claim
                    if (user == null)
                    {
                        ModelState.AddModelError("", "Người dùng không tồn tại.");
                        return View(model);
                    }

                    // Kiểm tra nếu người dùng bị khóa
                    if (user is SaleStaff && !((SaleStaff)user).IsActive) // Kiểm tra SaleStaff có IsActive là false không
                    {
                        ModelState.AddModelError("", "Tài khoản của bạn đã bị khóa.");
                        return View(model);
                    }

                    var roles = await _userManager.GetRolesAsync(user); // Lấy tất cả vai trò của người dùng

                    // Tạo các claims cho người dùng
                    var claims = new List<Claim>
                    {
                        // Kiểm tra nếu các giá trị không null
                        new Claim(ClaimTypes.NameIdentifier, user.Id?.ToString() ?? "UnknownId"),

                        new Claim(ClaimTypes.Name, user.UserName ?? "UnknownName"),

                        new Claim(ClaimTypes.Email, user.Email ?? "UnknownEmail")

                    };

                    // Thêm thông tin về role vào claims
                    foreach (var role in roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role)); // Thêm mỗi role vào claims
                    }

                    // Tạo ClaimsIdentity với các claims
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    // Đăng nhập người dùng với các claims này
                    await _signInManager.SignInAsync(user, isPersistent: model.RememberMe);

                    // Điều hướng người dùng theo vai trò
                    if (roles.Contains(ConstHelper.RoleManager))
                    {
                        return RedirectToAction("Index", "AdminOrders");
                    }
                    else if (roles.Contains(ConstHelper.RoleSaleStaff))
                    {
                        return RedirectToAction("Index", "AdminOrders");
                    }
                    else if (roles.Contains(ConstHelper.RoleCustomer))
                    {
                        // Nếu có returnUrl, điều hướng đến URL đó
                        return Redirect(returnUrl ?? "/Home/Index");
                    }

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
            }
            return View(model);
        }




        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

    }
}
