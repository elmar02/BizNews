using BizNews.Data;
using BizNews.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BizNews.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public FooterViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = _context.Categories.ToList();
            var popularNews = _context.Articles
                .Include(x => x.Category)
                .Where(x => x.IsDeleted == false)
                .Where(x => x.Ishidden == false)
                .OrderByDescending(x => x.ViewCount)
                .Take(3)
                .ToList();
            FooterVM footerVM = new()
            {
                Categories = categories,
                PopularNews = popularNews
            };
            return View("Footer", footerVM);
        }
    }
}
