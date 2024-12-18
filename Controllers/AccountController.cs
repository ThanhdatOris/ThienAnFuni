﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThienAnFuni.Models;
using ThienAnFuni.ViewModels;
using ThienAnFuni.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.AspNetCore.Authentication;

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



        //Quên mật khẩu:
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                // Nếu email không hợp lệ, thông báo lỗi.
                ModelState.AddModelError("", "Email không hợp lệ.");
                return View();
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                // Tạo token cho người dùng và gửi qua email.
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetLink = Url.Action("ResetPassword", "Account", new { token, email = email }, Request.Scheme);

                // Gửi email với liên kết reset mật khẩu (sử dụng dịch vụ gửi email của bạn).
                // Ví dụ: gửi email qua một dịch vụ như SendGrid, SMTP...

                // Thông báo cho người dùng về liên kết đã được gửi.
                TempData["SuccessMessage"] = "Một liên kết để đặt lại mật khẩu đã được gửi đến email của bạn.";
                return RedirectToAction("Login", "Account");
            }
            // Nếu không tìm thấy người dùng, thông báo lỗi.
            ModelState.AddModelError("", "Email không tồn tại.");
            return View();
        }





        // Đặt lại mật khẩu
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            // Kiểm tra token và email có hợp lệ hay không
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                // Nếu không hợp lệ, chuyển hướng về trang chủ
                return RedirectToAction("Index", "Home");
            }

            // Kiểm tra xem email có tồn tại trong hệ thống hay không
            var user = _userManager.FindByEmailAsync(email).Result;
            if (user == null)
            {
                // Nếu email không tồn tại, chuyển hướng về trang chủ
                return RedirectToAction("Index", "Home");
            }

            // Nếu hợp lệ, truyền token và email vào View
            ViewBag.Token = token;
            ViewBag.Email = email;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string token, string email, string password, string confirmPassword)
        {
            // Kiểm tra xem mật khẩu có hợp lệ không và có khớp với nhau không
            if (string.IsNullOrEmpty(password) || password.Length < 6 || password != confirmPassword)
            {
                ModelState.AddModelError("", "Mật khẩu không khớp hoặc không hợp lệ.");
                return View();
            }

            // Kiểm tra mật khẩu có đủ mạnh không (ví dụ: cần có ít nhất một chữ cái, một chữ số, một ký tự đặc biệt)
            if (!password.Any(char.IsDigit) || !password.Any(char.IsLetter) || !password.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                ModelState.AddModelError("", "Mật khẩu cần có ít nhất một chữ cái, một số và một ký tự đặc biệt.");
                return View();
            }

            // Tìm kiếm người dùng theo email
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                // Đặt lại mật khẩu
                var result = await _userManager.ResetPasswordAsync(user, token, password);
                if (result.Succeeded)
                {
                    // Thành công, chuyển đến trang xác nhận
                    return RedirectToAction("ResetPasswordConfirmation");
                }
                else
                {
                    // Thêm lỗi từ kết quả nếu có
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            else
            {
                // Nếu không tìm thấy người dùng, trả về thông báo lỗi
                ModelState.AddModelError("", "Không tìm thấy người dùng với email này.");
            }

            return View();
        }




        // Profile người dùng
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account"); // Chuyển hướng đến trang đăng nhập nếu chưa đăng nhập
            }

            // Lấy người dùng hiện tại từ Identity
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account"); // Nếu không tìm thấy người dùng, chuyển hướng đến trang đăng nhập
            }

            // Trả về view với đối tượng user
            return View(user);
        }


        [HttpPost]
        public async Task<IActionResult> Profile(string UserName, string FullName, string PhoneNumber, string Address, string Gender, DateTime? DateOfBirth, string OldPassword, string NewPassword, string ConfirmPassword)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User); // Lấy người dùng hiện tại từ Identity

                if (user == null)
                {
                    return RedirectToAction("Login", "Account"); // Nếu không tìm thấy người dùng, chuyển đến trang đăng nhập
                }

                // Cập nhật thông tin người dùng
                user.UserName = UserName;
                user.FullName = FullName;
                user.PhoneNumber = PhoneNumber;
                user.Address = Address;
                user.Gender = Gender;
                user.DateOfBirth = DateOfBirth;

                // Kiểm tra xem người dùng có cập nhật mật khẩu không
                if (!string.IsNullOrEmpty(NewPassword))
                {
                    if (NewPassword == ConfirmPassword)
                    {
                        // Đổi mật khẩu người dùng
                        var passwordChangeResult = await _userManager.ChangePasswordAsync(user, OldPassword, NewPassword);
                        if (!passwordChangeResult.Succeeded)
                        {
                            // Hiển thị lỗi nếu thay đổi mật khẩu không thành công
                            foreach (var error in passwordChangeResult.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                            return View(user); // Trả về view nếu có lỗi
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Mật khẩu mới và xác nhận mật khẩu không khớp.");
                        return View(user); // Trả về view nếu mật khẩu không khớp
                    }
                }

                // Lưu thay đổi người dùng
                var updateResult = await _userManager.UpdateAsync(user);

                if (updateResult.Succeeded)
                {
                    TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                    return RedirectToAction("Profile"); // Quay lại trang profile sau khi cập nhật thành công
                }

                // Hiển thị lỗi nếu cập nhật không thành công
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(UserName); // Trả về view với người dùng nếu có lỗi
        }

        // Phương thức để thay đổi mật khẩu
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View(); // Trả về trang đổi mật khẩu
        }


        [HttpPost]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            // Kiểm tra nếu mật khẩu mới và xác nhận mật khẩu không khớp
            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("", "Mật khẩu mới và xác nhận mật khẩu không khớp.");
                return View();
            }

            // Lấy người dùng hiện tại
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError("", "Không tìm thấy người dùng.");
                return View();
            }

            // Thay đổi mật khẩu của người dùng
            var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Thay đổi mật khẩu thành công!";
                return RedirectToAction("Profile");
            }

            // Nếu có lỗi, hiển thị lỗi
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View();
        }
    }

}
