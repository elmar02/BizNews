using BizNews.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BizNews.ViewComponents
{
    public class AdvertViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public AdvertViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var advert = _context.Adverts.FirstOrDefault();
            return View("Advert", advert);
        }
    }
}
