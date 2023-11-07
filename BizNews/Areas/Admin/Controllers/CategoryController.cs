using BizNews.Data;
using BizNews.Helper;
using BizNews.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BizNews.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    [Authorize(Roles = "Admin,Admin Editor")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var categories = _context.Categories.ToList();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(string categoryName)
        {
            try
            {
                if (categoryName == null)
                {
                    return View();
                }
                var category = _context.Categories.FirstOrDefault(x => x.CategoryName == categoryName);
                if (category != null)
                {
                    ModelState.AddModelError("Error", "This category name is already exist");
                    return View();
                }

                Category newCategory = new()
                {
                    CategoryName = categoryName,
                    SeoUrl = SeoHelper.ConverToSeo(categoryName)
                };
                _context.Categories.Add(newCategory);
                _context.SaveChanges();
                return Redirect("/admin/category");
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
            var category = _context.Categories.FirstOrDefault(x => x.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        public IActionResult Delete(Category category)
        {
            _context.Categories.Remove(category);
            _context.SaveChanges();
            return Redirect("/admin/category");
        }
    }
}
