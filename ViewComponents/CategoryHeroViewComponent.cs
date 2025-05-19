using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThienAnFuni.Models;

namespace ThienAnFuni.ViewComponents
{
    public class CategoryHeroViewComponent : ViewComponent
    {
        private readonly TAF_DbContext _context;

        public CategoryHeroViewComponent(TAF_DbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _context.Categories
                .Where(c => c.IsActive && c.ParentId == null)
                .Include(c => c.SubCategories)
                .ToListAsync();

            return View(categories);
        }

        //public IViewComponentResult Invoke()
        //{
        //    var categories = _context.Categories
        //        .Where(c => c.IsActive && c.ParentId == null)
        //        .Include(c => c.SubCategories)
        //        .ToList();

        //    return View(categories);
        //}


    }
}
