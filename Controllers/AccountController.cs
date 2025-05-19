using Microsoft.AspNetCore.Identity;
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
        public IActionResult Error404()
        {
            return View("404NotFound");
        }


        // Đăng kí
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

                //// Đếm số lượng khách hàng hiện tại
                //var customerCount = await _userManager.Users.OfType<Customer>().CountAsync();

                // Lấy mã khách hàng tiếp theo
                int customerCount = await _userManager.Users.OfType<Customer>().CountAsync();
                int customerCode = customerCount + 1; // Mã khách hàng tự động tăng

                //// Sinh mã khách hàng tự động tăng
                //int customerCode = customerCount + 1;

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

                    // Có thể hiển thị mã khách hàng mới tạo nếu cần thiết
                    ViewBag.CustomerCode = customerCode;

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


        // Đăng nhập
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            // Lưu lại URL người dùng muốn truy cập sau khi đăng nhập
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        #region Old Login
        //public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);
        //        if (result.Succeeded)
        //        {
        //            var user = await _userManager.FindByNameAsync(model.Username);

        //            // Kiểm tra nếu user không null và đảm bảo không có giá trị null cho các claim
        //            if (user == null)
        //            {
        //                ModelState.AddModelError("", "Người dùng không tồn tại.");
        //                return View(model);
        //            }

        //            // Kiểm tra nếu người dùng bị khóa
        //            if (user is SaleStaff && !((SaleStaff)user).IsActive) // Kiểm tra SaleStaff có IsActive là false không
        //            {
        //                ModelState.AddModelError("", "Tài khoản của bạn đã bị khóa.");
        //                return View(model);
        //            }

        //            var roles = await _userManager.GetRolesAsync(user); // Lấy tất cả vai trò của người dùng

        //            // Tạo các claims cho người dùng
        //            var claims = new List<Claim>
        //            {
        //                // Kiểm tra nếu các giá trị không null
        //                new Claim(ClaimTypes.NameIdentifier, user.Id?.ToString() ?? "UnknownId"),

        //                new Claim(ClaimTypes.Name, user.UserName ?? "UnknownName"),

        //                new Claim(ClaimTypes.Email, user.Email ?? "UnknownEmail")

        //            };

        //            // Thêm thông tin về role vào claims
        //            foreach (var role in roles)
        //            {
        //                claims.Add(new Claim(ClaimTypes.Role, role)); // Thêm mỗi role vào claims
        //            }

        //            // Tạo ClaimsIdentity với các claims
        //            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        //            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        //            // Đăng nhập người dùng với các claims này
        //            await _signInManager.SignInAsync(user, isPersistent: model.RememberMe);

        //            // Điều hướng người dùng theo vai trò
        //            if (roles.Contains(ConstHelper.RoleManager))
        //            {
        //                return RedirectToAction("Index", "Dashboard");
        //            }
        //            else if (roles.Contains(ConstHelper.RoleSaleStaff))
        //            {
        //                return RedirectToAction("Index", "Dashboard");
        //            }
        //            else if (roles.Contains(ConstHelper.RoleCustomer))
        //            {
        //                // Nếu có returnUrl, điều hướng đến URL đó
        //                return Redirect(returnUrl ?? "/Home/Index");
        //            }

        //            return RedirectToAction("Index", "Home");
        //        }
        //        ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
        //    }
        //    return View(model);
        //}
        #endregion
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Đăng nhập người dùng
            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(model.Username);

                // Kiểm tra người dùng có bị khóa không
                if (user is SaleStaff saleStaff && !saleStaff.IsActive)
                {
                    await _signInManager.SignOutAsync();
                    ModelState.AddModelError("", "Tài khoản của bạn đã bị khóa.");
                    return View(model);
                }

                // Lấy vai trò của người dùng
                var roles = await _userManager.GetRolesAsync(user);

                // Điều hướng dựa vào vai trò
                if (roles.Contains(ConstHelper.RoleManager))
                {
                    return RedirectToAction("Index", "Dashboard");
                }
                if (roles.Contains(ConstHelper.RoleSaleStaff))
                {
                    return RedirectToAction("Index", "Dashboard");
                }
                if (roles.Contains(ConstHelper.RoleCustomer))
                {
                    return Redirect(returnUrl ?? "/Home/Index");
                }

                // Nếu không có vai trò phù hợp
                await _signInManager.SignOutAsync();
                ModelState.AddModelError("", "Quyền truy cập không hợp lệ.");
                return View(model);
            }

            // Thông báo khi đăng nhập thất bại
            ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
            return View(model);
        }


        // Đăng xuất
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


        //Quên mật khẩu
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
                return RedirectToAction("Login", "Account");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(user);
        }


        [HttpPost]
        #region Old Profile
        //public async Task<IActionResult> Profile(User model, string oldPassword, string newPassword, string confirmPassword)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await _userManager.GetUserAsync(User);
        //        if (user == null)
        //        {
        //            return RedirectToAction("Login", "Account");
        //        }

        //        // Cập nhật thông tin người dùng
        //        user.FullName = model.FullName;
        //        user.PhoneNumber = model.PhoneNumber;
        //        user.Address = model.Address;
        //        user.Gender = model.Gender;
        //        user.DateOfBirth = model.DateOfBirth;
        //        user.Email = model.Email;


        //        // Cập nhật thông tin cơ bản
        //        var result = await _userManager.UpdateAsync(user);
        //        if (!result.Succeeded)
        //        {
        //            foreach (var error in result.Errors)
        //            {
        //                ModelState.AddModelError("", error.Description);
        //            }
        //            return View(model);
        //        }

        //        // Kiểm tra và thay đổi mật khẩu
        //        if (!string.IsNullOrWhiteSpace(newPassword))
        //        {
        //            if (newPassword != confirmPassword)
        //            {
        //                TempData["ConfirmPasswordError"] = "Mật khẩu mới và xác nhận mật khẩu không khớp.";
        //                return View(model);
        //            }


        //            var passwordChangeResult = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
        //            if (!passwordChangeResult.Succeeded)
        //            {
        //                foreach (var error in passwordChangeResult.Errors)
        //                {
        //                    foreach (var error in result.Errors)
        //                    {
        //                        ModelState.AddModelError("", error.Description);
        //                    }
        //                    return View(model);
        //                }

        //                // Kiểm tra và thay đổi mật khẩu
        //                if (!string.IsNullOrWhiteSpace(newPassword))
        //                {
        //                    if (newPassword != confirmPassword)
        //                    {
        //                        ModelState.AddModelError("", "Mật khẩu mới và xác nhận mật khẩu không khớp.");
        //                        return View(model);
        //                    }

        //                    var passwordChangeResult = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
        //                    if (!passwordChangeResult.Succeeded)
        //                    {
        //                        foreach (var error in passwordChangeResult.Errors)
        //                        {
        //                            ModelState.AddModelError("", error.Description);
        //                        }
        //                        return View(model);
        //                    }
        //                }

        //                TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
        //                return RedirectToAction("Profile");
        //            }

        //            return View(model); // Trả về view với model nếu có lỗi
        //        }

        //        TempData["SuccessMessage"] = "Thông tin đã được cập nhật!";
        //        return RedirectToAction("Profile");
        //    }

        //    return View(model); // Trả về view với model nếu có lỗi
        //}
        #endregion
        public async Task<IActionResult> Profile(User model, string oldPassword, string newPassword, string confirmPassword)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Cập nhật thông tin cá nhân
            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;
            user.Gender = model.Gender;
            user.DateOfBirth = model.DateOfBirth;
            user.Email = model.Email;

            // Cập nhật thông tin cơ bản
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }

            // Kiểm tra và cập nhật mật khẩu nếu người dùng muốn đổi mật khẩu
            if (!string.IsNullOrWhiteSpace(newPassword))
            {
                if (newPassword != confirmPassword)
                {
                    ModelState.AddModelError("", "Mật khẩu mới và xác nhận mật khẩu không khớp.");
                    return View(model);
                }

                if (string.IsNullOrWhiteSpace(oldPassword))
                {
                    ModelState.AddModelError("", "Vui lòng nhập mật khẩu cũ để thay đổi mật khẩu mới.");
                    return View(model);
                }

                var passwordChangeResult = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
                if (!passwordChangeResult.Succeeded)
                {
                    foreach (var error in passwordChangeResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }
            }

            TempData["SuccessMessage"] = "Thông tin đã được cập nhật!";
            return RedirectToAction("Profile");
        }

    }

}
