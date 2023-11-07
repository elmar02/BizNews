using BizNews.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BizNews.ViewComponents
{
    public class NotifyViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        public NotifyViewComponent(AppDbContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            int count = 0;
            var userId = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (userId != null)
            {
                count = _context.Notifies
                .Where(x => x.UserId == userId)
                .Where(x => !x.IsSeen).ToList().Count;
            }
            return View("Notify", count);
        }
    }
}
