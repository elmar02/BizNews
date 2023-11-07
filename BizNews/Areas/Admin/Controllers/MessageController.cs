using BizNews.Data;
using BizNews.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BizNews.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    [Authorize(Roles = "Admin,Admin Editor")]
    public class MessageController : Controller
    {
        private readonly AppDbContext _context;

        public MessageController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var messages = _context.Contacts.OrderByDescending(x=>x.CreatedDate).ToList();
            var messageList = DeepCopyHelper.DeepCopy(messages);
            foreach (var message in messages)
            {
                message.IsSeen = true;
            }
            _context.Contacts.UpdateRange(messages);
            _context.SaveChanges();
            return View(messageList);
        }
    }
}
