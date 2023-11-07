using BizNews.Data;
using BizNews.Models;
using BizNews.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BizNews.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var articles = _context.Articles
                .Where(x => x.IsPublished == true)
                .Where(x => x.IsDeleted == false)
                .Where(x => x.Ishidden == false)
                .Include(x=>x.Category)
                .Include(x=>x.User)
                .Include(x=>x.Comments)
                .OrderByDescending(x=>x.CreatedDate);
            var advert = _context.Adverts.FirstOrDefault();
            var tags = _context.Tags.ToList();
            HomeVM homeVM = new HomeVM()
            {
                ArticlesInSlider = articles.Where(x => x.IsInSlider).Take(7).ToList(),
                FeaturedNews = articles.Where(x => x.IsFeatured).Take(10).ToList(),
                BreakingNews = articles.Take(2).ToList(),
                Advert = advert,
                Tags = tags,
                TrandingNews = articles.OrderByDescending(x => x.ViewCount).ThenByDescending(x => x.Comments.Count).Take(5).ToList(),
                LatestNews = articles.Take(6).ToList()
            };
            return View(homeVM);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}