using BizNews.Data;
using BizNews.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BizNews.Controllers
{
    public class NotifyController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public NotifyController(AppDbContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }

        [Authorize]
        public IActionResult Index()
        {
            var userId = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (userId == null)
            {
                return Redirect("/auth/login?ReturnUrl=%2Fnotify");
            }
            var notifies = _context.Notifies
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedDate)
                .ToList();
            var notifyList = DeepCopyHelper.DeepCopy(notifies);
            foreach (var notify in notifies)
            {
                notify.IsSeen = true;
            }
            _context.Notifies.UpdateRange(notifies);
            _context.SaveChanges();
            return View(notifyList);
        }
    }
}
