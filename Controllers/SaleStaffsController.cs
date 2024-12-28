using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ThienAnFuni.Helpers;
using ThienAnFuni.Models;

namespace ThienAnFuni.Controllers
{
    [Authorize(Roles = ConstHelper.RoleManager)]
    public class SaleStaffsController : Controller
    {
        private readonly TAF_DbContext _context;
        private readonly UserManager<User> _userManager;

        public SaleStaffsController(TAF_DbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: SaleStaffs
        public async Task<IActionResult> Index()
        {
            ViewData["ActiveMenu"] = "SaleStaff";

            return View(await _context.SaleStaffs.Where(s => s.IsActive == true).ToListAsync());
        }

        public async Task<IActionResult> ListDeleted()
        {
            ViewData["ActiveMenu"] = "SaleStaff";

            return View(await _context.SaleStaffs.Where(s => s.IsActive == false).ToListAsync());
        }

        // GET: SaleStaffs/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            ViewData["ActiveMenu"] = "SaleStaff";

            if (id == null)
            {
                return NotFound();
            }

            var saleStaff = await _context.SaleStaffs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (saleStaff == null)
            {
                return NotFound();
            }

            return View(saleStaff);
        }

        // GET: SaleStaffs/Create
        public IActionResult Create()
        {
            ViewData["ActiveMenu"] = "SaleStaff";

            return View();
        }

        // POST: SaleStaffs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CitizenId,IssuingDate,IssuingPlace,StartDate,EndDate,Degree,Id,FullName,UserName,PhoneNumber,Address,Gender,DateOfBirth,Password")] SaleStaff saleStaff)
        {
            ViewData["ActiveMenu"] = "SaleStaff";

            if (ModelState.IsValid)
            {
                // Kiểm tra nếu CCCD hoặc số điện thoại đã tồn tại trong bảng Users
                var existingStaff = await _context.Users
                    .Where(u => (u.CitizenId == saleStaff.CitizenId || u.PhoneNumber == saleStaff.PhoneNumber) && u is SaleStaff)
                    .FirstOrDefaultAsync();

                if (existingStaff != null)
                {
                    // Thêm thông báo lỗi vào TempData để hiển thị trong view
                    TempData["ErrorMessage"] = "Nhân viên với CCCD hoặc số điện thoại này đã tồn tại.";
                    return RedirectToAction("Create");
                }

                // Cấu hình các thuộc tính mặc định
                saleStaff.IsActive = true;
                saleStaff.StartDate = DateTime.Now;

                // Tạo tài khoản thông qua UserManager sẽ chính xác và tiện lợi hơn
                var result = await _userManager.CreateAsync(saleStaff, saleStaff.PhoneNumber); // Mật khẩu mặc định là số điện thoại

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(saleStaff, ConstHelper.RoleSaleStaff);
                    return RedirectToAction(nameof(Index));
                }

                // Truyền lỗi bằng kỹ thuật TempData
                TempData["ErrorMessage"] = string.Join("<br>", result.Errors.Select(e => e.Description));
            }
            return View(saleStaff);
        }


        // GET: SaleStaffs/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {
            ViewData["ActiveMenu"] = "SaleStaff";

            if (id == null)
            {
                return NotFound();
            }

            var saleStaff = await _context.SaleStaffs.FindAsync(id);
            if (saleStaff == null)
            {
                return NotFound();
            }
            return View(saleStaff);
        }

        // POST: SaleStaffs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("CitizenId,IssuingDate,IssuingPlace,StartDate,EndDate,Degree,Id,FullName,Username,PhoneNumber,Address,Gender,DateOfBirth")] SaleStaff saleStaff)
        {
            ViewData["ActiveMenu"] = "SaleStaff";

            if (id != saleStaff.Id)
            {
                return NotFound();
            }

            // Lấy thông tin nhân viên bán hàng từ cơ sở dữ liệu
            var existingStaff = await _context.SaleStaffs.FindAsync(id);
            if (existingStaff == null)
            {
                return NotFound();
            }

            // Xóa trường Password khỏi ModelState để không ghi đè
            ModelState.Remove("Password");

            if (ModelState.IsValid)
            {
                try
                {
                    // Cập nhật các thông tin từ form, trừ Password
                    existingStaff.CitizenId = saleStaff.CitizenId;
                    existingStaff.IssuingDate = saleStaff.IssuingDate;
                    existingStaff.IssuingPlace = saleStaff.IssuingPlace;
                    existingStaff.StartDate = saleStaff.StartDate;
                    existingStaff.EndDate = saleStaff.EndDate;
                    existingStaff.Degree = saleStaff.Degree;
                    existingStaff.FullName = saleStaff.FullName;
                    existingStaff.UserName = saleStaff.UserName;
                    existingStaff.PhoneNumber = saleStaff.PhoneNumber;
                    existingStaff.Address = saleStaff.Address;
                    existingStaff.Gender = saleStaff.Gender;
                    existingStaff.DateOfBirth = saleStaff.DateOfBirth;

                    _context.Update(existingStaff);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SaleStaffExists(saleStaff.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(saleStaff);
        }

        // GET: SaleStaffs/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            ViewData["ActiveMenu"] = "SaleStaff";

            if (id == null)
            {
                return NotFound();
            }

            var saleStaff = await _context.SaleStaffs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (saleStaff == null)
            {
                return NotFound();
            }

            return View(saleStaff);
        }

        // POST: SaleStaffs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            ViewData["ActiveMenu"] = "SaleStaff";

            var saleStaff = await _context.SaleStaffs.FindAsync(id);
            if (saleStaff != null)
            {
                saleStaff.IsActive = false;
                saleStaff.EndDate = DateTime.Now;
                _context.SaleStaffs.Update(saleStaff);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        private bool SaleStaffExists(string id)
        {
            ViewData["ActiveMenu"] = "SaleStaff";

            return _context.SaleStaffs.Any(e => e.Id == id);
        }
    }
}
