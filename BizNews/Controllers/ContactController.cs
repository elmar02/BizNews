using BizNews.Data;
using BizNews.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BizNews.Controllers
{
    public class ContactController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<User> _userManager;

        public ContactController(AppDbContext context, IHttpContextAccessor contextAccessor, UserManager<User> userManager)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(Contact contact)
        {
            try
            {
                if (contact == null)
                {
                    return NotFound();
                }
                if (User.Identity.IsAuthenticated)
                {
                    var userId = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    var user = await _userManager.FindByIdAsync(userId);
                    contact.FirstName = user.FirstName;
                    contact.Email = user.Email;
                }
                contact.IsSeen = false;
                contact.CreatedDate = DateTime.Now;
                _context.Contacts.Add(contact);
                _context.SaveChanges();
                return Redirect("/contact");
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
    }
}
