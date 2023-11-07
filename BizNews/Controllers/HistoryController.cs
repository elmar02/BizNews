using BizNews.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BizNews.Controllers
{
    public class HistoryController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        public HistoryController(AppDbContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }

        public IActionResult Index()
        {
            var userId = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var history = _context.HistoryRecords
                .Include(x=>x.Article)
                .Where(x=>x.UserId == userId)
                .OrderByDescending(x=>x.SeenDate)
                .ToList();
            if (history.Count > 20)
            {
                _context.HistoryRecords.RemoveRange(history.Skip(20));
                _context.SaveChanges();
            }
            return View(history.Take(20).ToList());
        }
    }
}
