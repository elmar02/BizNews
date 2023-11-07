using BizNews.Data;
using BizNews.DTOs;
using BizNews.Helper;
using BizNews.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BizNews.Controllers
{
    [Route("category")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var categories = _context.Categories.ToList();
            return View(categories);
        }

        [HttpGet("{categoryName}")]
        public IActionResult CategoryDetail(int page,string categoryName) 
        {
            var articles = _context.Articles
                .Where(x => x.IsPublished == true)
                .Where(x => x.IsDeleted == false)
                .Where(x => x.Ishidden == false)
                .Include(x => x.Category)
                .Include(x => x.ArticleTags)
                .ThenInclude(x => x.Tag)
                .Include(x => x.User)
                .Include(x => x.Comments)
                .OrderByDescending(x => x.CreatedDate)
                .Where(x=>x.Category.SeoUrl == categoryName.ToLower())
                .ToList();
            var tags = _context.Tags.ToList();

            CategoryTagVM categoryTagVM = new()
            {
                Articles = articles,
                PageDTO = articles.CreatePageDTO(page),
                Tags = tags,
                Advert = _context.Adverts.FirstOrDefault(),
                TrandingNews = _context.Articles
                .Include(x=>x.Category)
                .OrderByDescending(x => x.ViewCount).ThenByDescending(x => x.Comments.Count).Take(5).ToList(),
                CategoryTagName = categoryName,
            };
            return View(categoryTagVM);
        }
    }
}
