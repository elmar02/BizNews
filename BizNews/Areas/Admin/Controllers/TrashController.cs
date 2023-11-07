using Azure;
using BizNews.Data;
using BizNews.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;

namespace BizNews.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    [Authorize(Roles = "Admin,Admin Editor")]
    public class TrashController : Controller
    {


        private readonly AppDbContext _context;

        public TrashController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var articles = _context.Articles
                .Include(x => x.Category)
                .Include(x => x.ArticleTags)
                .ThenInclude(x => x.Tag)
                .Where(x => x.IsDeleted == true)
                .OrderByDescending(x => x.DeletedDate)
                .ToList();
            var thirtyDaysAgo = DateTime.Now.AddDays(-30);
            var deletedArticles = articles
            .Where(x => x.DeletedDate < thirtyDaysAgo)
            .ToList();
            _context.Articles.RemoveRange(deletedArticles);
            _context.SaveChanges();
            return View(articles);
        }

        [HttpGet]
        public IActionResult Restore(int id)
        {
            try
            {
                var article = _context.Articles.FirstOrDefault(x => x.Id == id);

                if (article == null)
                {
                    return NotFound();
                }
                return View(article);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public IActionResult Restore(Article article)
        {
            try
            {
                var deletedArticle = _context.Articles.FirstOrDefault(x => x.Id == article.Id);
                if (deletedArticle == null)
                {
                    return View(deletedArticle);
                }
                deletedArticle.IsDeleted = false;
                _context.Articles.Update(deletedArticle);
                _context.SaveChanges();
                return Redirect("/admin/trash");
            }
            catch (Exception)
            {
                return View();
            }
        }

        [HttpGet]
        public IActionResult HardDelete(int id)
        {
            try
            {
                var article = _context.Articles.FirstOrDefault(x => x.Id == id);

                if (article == null)
                {
                    return NotFound();
                }
                return View(article);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public IActionResult HardDelete(Article article)
        {
            _context.Articles.Remove(article);
            _context.SaveChanges();
            return Redirect("/admin/trash");
        }
    }
}
