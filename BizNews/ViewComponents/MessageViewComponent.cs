using BizNews.Data;
using Microsoft.AspNetCore.Mvc;

namespace BizNews.ViewComponents
{
    public class MessageViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public MessageViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var count = _context.Contacts.Where(x=>!x.IsSeen).ToList().Count;
            return View("Message", count);
        }
    }
}
