using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ThienAnFuni.Models;

namespace ThienAnFuni.Controllers
{
    [Authorize] //Yêu cầu người dùng phải đăng nhập mới có quyền truy cập
    public class OrdersController : Controller
    {
        private readonly TAF_DbContext _context;

        public OrdersController(TAF_DbContext context)
        {
            _context = context;
        }


        // GET: Orders
        public async Task<IActionResult> Index()
        {
            // Lấy thông tin của người dùng đã đăng nhập
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Lấy danh sách đơn hàng thuộc về người dùng hiện tại
            var tAF_DbContext = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.SaleStaff)
                .Where(o => o.CustomerId == userId) // Chỉ lấy đơn hàng của người dùng hiện tại
                .OrderByDescending(o => o.Id);

            return View(await tAF_DbContext.ToListAsync());
        }

        // Hiển thị thành công
        public IActionResult OrderSuccess()
        {
            return View();
        }


        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Lấy thông tin của người dùng hiện tại
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Tìm kiếm đơn hàng và kiểm tra xem nó có thuộc về người dùng hiện tại không
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.SaleStaff)
                .Include(o => o.OrderDetails)
                    .ThenInclude(o => o.Product)
                        .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(m => m.Id == id && m.CustomerId == userId); // Kiểm tra thêm CustomerId

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id");
            ViewData["SaleStaffId"] = new SelectList(_context.SaleStaffs, "Id", "Id");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CustomerPhoneNumber,TotalPrice,TotalQuantity,Note,Address,OrderDate,OrderStatus,PaymentMethod,PaymentStatus,InvoiceNumber,InvoiceDate,CustomerId,SaleStaffId")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id", order.CustomerId);
            ViewData["SaleStaffId"] = new SelectList(_context.SaleStaffs, "Id", "Id", order.SaleStaffId);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id", order.CustomerId);
            ViewData["SaleStaffId"] = new SelectList(_context.SaleStaffs, "Id", "Id", order.SaleStaffId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CustomerPhoneNumber,TotalPrice,TotalQuantity,Note,Address,OrderDate,OrderStatus,PaymentMethod,PaymentStatus,InvoiceNumber,InvoiceDate,CustomerId,SaleStaffId")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id", order.CustomerId);
            ViewData["SaleStaffId"] = new SelectList(_context.SaleStaffs, "Id", "Id", order.SaleStaffId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.SaleStaff)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
