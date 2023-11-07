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
    public class TagController : Controller
    {
        private readonly AppDbContext _context;

        public TagController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var tags = _context.Tags.ToList();
            return View(tags);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(string tagName)
        {
            try
            {
                if (tagName == null)
                {
                    return View();
                }
                var tag = _context.Tags.FirstOrDefault(x => x.TagName == tagName);
                if (tag != null)
                {
                    ModelState.AddModelError("Error", "This tag name is already exist");
                    return View();
                }

                Tag newTag = new()
                {
                    TagName = tagName,
                    SeoUrl = SeoHelper.ConverToSeo(tagName)
                };
                _context.Tags.Add(newTag);
                _context.SaveChanges();
                return Redirect("/admin/tag");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return View();
            }
        }

        public IActionResult Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var tag = _context.Tags.FirstOrDefault(x => x.Id == id);
            if (tag == null)
            {
                return NotFound();
            }
            return View(tag);
        }
        [HttpPost]
        public IActionResult Delete(Tag tag)
        {
            _context.Tags.Remove(tag);
            _context.SaveChanges();
            return Redirect("/admin/tag");
        }
    }
}
