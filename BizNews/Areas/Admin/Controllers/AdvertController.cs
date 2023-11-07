using BizNews.Data;
using BizNews.Helper;
using BizNews.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BizNews.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    [Authorize(Roles = "Admin,Admin Editor")]
    public class AdvertController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public AdvertController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index()
        {
            var advert = _context.Adverts.FirstOrDefault();
            return View(advert);
        }

        public IActionResult Create()
        {
            var advert = _context.Adverts.FirstOrDefault();
            if (advert != null)
            {
                return Redirect("/admin/advert");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Advert advert,IFormFile Photo)
        {
            try
            {
                if (advert == null)
                {
                    return NotFound();
                }
                if (Photo == null)
                {
                    ViewData["Error"] = "Please upload photo";
                    return View();
                }
                advert.PhotoUrl = await Photo.SaveFileAsync(_env.WebRootPath);
                advert.ClickedCount = 0;
                _context.Adverts.Add(advert);
                _context.SaveChanges();
                return Redirect("/admin/advert");
            }
            catch (Exception)
            {
                return View();
            }
        }

        public IActionResult Edit()
        {
            var advert = _context.Adverts.FirstOrDefault();
            if (advert == null)
            {
                return NotFound();
            }
            return View(advert);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Advert advert, IFormFile Photo)
        {
            try
            {
                var updatedAdvert = _context.Adverts.FirstOrDefault();

                if (updatedAdvert == null)
                {
                    return NotFound();
                }
                if (Photo != null)
                {
                    updatedAdvert.PhotoUrl = await Photo.SaveFileAsync(_env.WebRootPath);
                }

                updatedAdvert.Link = advert.Link;
                _context.Adverts.Update(updatedAdvert);
                _context.SaveChanges();
                return Redirect("/admin/advert");
            }
            catch (Exception)
            {
                var updatedAdvert = _context.Adverts.FirstOrDefault();
                return View(updatedAdvert);
            }
        }

        public IActionResult Delete()
        {
            var advert = _context.Adverts.FirstOrDefault();
            if (advert == null)
            {
                return NotFound();
            }
            return View();
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                var advert = _context.Adverts.FirstOrDefault();
                if (advert == null)
                {
                    return NotFound();
                }

                _context.Adverts.Remove(advert);
                _context.SaveChanges();
                return Redirect("/admin/advert");
            }
            catch (Exception)
            {
                return View();
            }
        }
    }
}
