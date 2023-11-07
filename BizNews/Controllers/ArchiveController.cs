using BizNews.Data;
using BizNews.DTOs;
using BizNews.Helper;
using BizNews.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BizNews.Controllers
{
    public class ArchiveController : Controller
    {
        private readonly AppDbContext _context;

        public ArchiveController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int year,int month,int day, int page)
        {
            List<int> dates = new List<int>();
            string url = "/archive";
            var articles = _context.Articles
                .Include(x => x.Category)
                .Include(x => x.ArticleTags)
                .ThenInclude(x => x.Tag)
                .Include(x => x.User)
                .Include(x => x.Comments)
                .Where(x => x.IsPublished == true)
                .Where(x => x.IsDeleted == false)
                .Where(x => x.Ishidden == false)
                .OrderByDescending(x => x.CreatedDate)
                .ToList();
            if (year == 0)
            {
                return NotFound();
            }
            articles = articles.Where(x => x.CreatedDate.Year == year).ToList();
            url += $"/{year}";
            dates.Add(year);
            if (month != 0)
            {
                articles = articles.Where(x => x.CreatedDate.Month == month).ToList();
                url += $"/{month}";
                dates.Add(month);
            }
            else
            {
                month = 1;
            }
            if (day != 0)
            {
                articles = articles.Where(x => x.CreatedDate.Day == day).ToList();
                url += $"/{day}";
                dates.Add(day);
            }
            else { day = 1; }
            var tags = _context.Tags.ToList();
            var archiveDate = new DateTime(2000, 1, 1);
            try
            {
                archiveDate = new DateTime(year, month, day);
            }
            catch (Exception)
            {
                
            }
            ArchiveVM archiveVM = new()
            {
                Articles = articles,
                PageDTO = articles.CreatePageDTO(page),
                Tags = tags,
                Advert = _context.Adverts.FirstOrDefault(),
                TrandingNews = _context.Articles
                .Where(x => x.IsPublished == true)
                .Where(x => x.IsDeleted == false)
                .Where(x => x.Ishidden == false)
                .Include(x => x.Category)
                .OrderByDescending(x => x.ViewCount).ThenByDescending(x => x.Comments.Count).Take(5).ToList(),
                Date = archiveDate,
                Url = url,
                Dates = dates
            };
            return View(archiveVM);
        }
    }
}
