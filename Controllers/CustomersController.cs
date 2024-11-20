using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThienAnFuni.Models;

namespace ThienAnFuni.Controllers
{
    public class CustomersController : Controller
    {
        private readonly TAF_DbContext _context;

        public CustomersController(TAF_DbContext context)
        {
            _context = context;
        }

        // Phương thức kiểm tra sự tồn tại của khách hàng
        private bool CustomerExists(string id)
        {
            ViewData["ActiveMenu"] = "Customer";
            return _context.Customers.Any(e => e.Id == id);
        }


        // GET: Customers
        public async Task<IActionResult> Index()
        {
            ViewData["ActiveMenu"] = "Customer";

            return View(await _context.Customers.Where(s => s.IsActive == true).ToListAsync());
        }

        // GET: Customers/Edit/5
        // phương thức này nhận 1 Id và trả về trang form chỉnh sửa với thông tin của khách hàng 
        public async Task<IActionResult> Edit(string? id)
        {
            ViewData["ActiveMenu"] = "Customer";
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(string id, [Bind("Id,FullName,Address,PhoneNumber,Email,DateOfBirth,Gender")] Customer customer)
        //{
        //    ViewData["ActiveMenu"] = "Customer";
        //    if (id != customer.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(customer);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!CustomerExists(customer.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        TempData["Success"] = "Cập nhật thành công!";
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(customer);
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,FullName,Address,PhoneNumber,Email,DateOfBirth,Gender")] Customer customer)
        {
            ViewData["ActiveMenu"] = "Customer";

            // Kiểm tra xem ID khách hàng trong URL có khớp với ID của model không
            if (id != customer.Id)
            {
                return NotFound(); // Nếu không khớp, trả về lỗi
            }

            // Lấy thông tin khách hàng từ cơ sở dữ liệu
            var existingCustomer = await _context.Customers.FindAsync(id);
            if (existingCustomer == null)
            {
                return NotFound(); // Nếu khách hàng không tồn tại trong cơ sở dữ liệu, trả về lỗi
            }

            // Cập nhật thông tin khách hàng
            existingCustomer.FullName = customer.FullName;
            existingCustomer.PhoneNumber = customer.PhoneNumber;
            existingCustomer.Address = customer.Address;
            existingCustomer.Email = customer.Email;
            existingCustomer.DateOfBirth = customer.DateOfBirth;
            existingCustomer.Gender = customer.Gender;

            // Cập nhật vào cơ sở dữ liệu
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(existingCustomer);
                    await _context.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.Id))
                    {
                        return NotFound(); // Nếu khách hàng không tồn tại, trả về lỗi
                    }
                    else
                    {
                        throw;
                    }
                }

                TempData["Success"] = "Cập nhật thành công!"; // Hiển thị thông báo thành công
                return RedirectToAction(nameof(Index)); // Chuyển hướng về trang danh sách
            }

            return View(customer); // Nếu không hợp lệ, quay lại form với lỗi
        }

    }
}
