using BizNews.Data;
using BizNews.DTOs;
using BizNews.Helper;
using BizNews.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BizNews.Controllers
{
    [Route("tag")]
    public class TagController : Controller
    {
        private readonly AppDbContext _context;

        public TagController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var tags = _context.Tags.ToList();
            return View(tags);
        }

        [HttpGet("{tagName}")]
        public IActionResult TagDetail(int page,string tagName) 
        {
            var articles = _context.Articles
                .Where(x => x.IsPublished == true)
                .Where(x => x.IsDeleted == false)
                .Where(x => x.Ishidden == false)
                .Include(x => x.Category)
                .Include(x => x.ArticleTags)
                .ThenInclude(x => x.Tag)
                .Where(x=> x.ArticleTags.Any(y=>y.Tag.SeoUrl == tagName.ToLower()))
                .Include(x => x.User)
                .Include(x => x.Comments)
                .OrderByDescending(x => x.CreatedDate)
                .ToList();

            var tags = _context.Tags.ToList();

            CategoryTagVM categoryTagVM = new()
            {
                Articles = articles,
                PageDTO = articles.CreatePageDTO(page),
                Tags = tags,
                Advert = _context.Adverts.FirstOrDefault(),
                TrandingNews = _context.Articles
                .Where(x => x.IsPublished == true)
                .Where(x => x.IsDeleted == false)
                .Where(x => x.Ishidden == false)
                .Include(x=>x.Category)
                .OrderByDescending(x => x.ViewCount).ThenByDescending(x => x.Comments.Count).Take(5).ToList(),
                CategoryTagName = tagName,
            };
            return View(categoryTagVM);
        }
    }
}
