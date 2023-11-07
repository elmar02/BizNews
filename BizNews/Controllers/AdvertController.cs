using BizNews.Data;
using Microsoft.AspNetCore.Mvc;

namespace BizNews.Controllers
{
    public class AdvertController : Controller
    {
        private readonly AppDbContext _context;

        public AdvertController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult ClickedAd()
        {
            var advert = _context.Adverts.FirstOrDefault();
            if (advert == null)
            {
                return NotFound();
            }
            advert.ClickedCount++;
            _context.Adverts.Update(advert);
            _context.SaveChanges();
            return Redirect(advert.Link);
        }
    }
}
