using BizNews.Data;
using BizNews.Helper;
using BizNews.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;

namespace BizNews.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    [Authorize(Roles = "Admin,Admin Editor")]
    public class ArticleController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;
        public ArticleController(AppDbContext context, IWebHostEnvironment env, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
            _contextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            var articles = _context.Articles
                .Include(x => x.Category)
                .Include(x => x.ArticleTags)
                .ThenInclude(x => x.Tag)
                .Where(x => x.IsDeleted == false)
                .OrderByDescending(x => x.CreatedDate)
                .ToList();
            return View(articles);
        }

        public IActionResult Create()
        {
            var categories = _context.Categories.ToList();
            var tags = _context.Tags.ToList();
            ViewData["tags"] = tags;
            ViewData["categories"] = categories;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Article article, List<int> tagIds, IFormFile Photo)
        {
            try
            {
                var categories = _context.Categories.ToList();
                var tags = _context.Tags.ToList();
                ViewData["tags"] = tags;
                ViewData["categories"] = categories;
                if (Photo == null)
                {
                    ViewData["Photo"] = "The Article Photo field is required.";
                    return View();
                }
                if (tagIds.Count == 0)
                {
                    ViewData["tagError"] = "Choose at least one tag";
                    return View();
                }
                article.PhotoUrl = await Photo.SaveFileAsync(_env.WebRootPath);
                article.CreatedDate = DateTime.Now;
                var userId =_contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                article.UserId = userId;
                var user = await _userManager.FindByIdAsync(userId);
                article.CreatedBy = user.UserName;
                article.ViewCount = 0;
                article.IsDeleted = false;
                article.SeoUrl = SeoHelper.ConverToSeo(article.Title);
                _context.Articles.Add(article);
                _context.SaveChanges();

                List<ArticleTag> articleTags = new();

                for (int i = 0; i < tagIds.Count; i++)
                {
                    ArticleTag articleTag = new()
                    {
                        ArticleId = article.Id,
                        TagId = tagIds[i],
                    };
                    articleTags.Add(articleTag);
                }
                _context.ArticleTags.AddRange(articleTags);
                _context.SaveChanges();
                return Redirect("/admin/article");
            }
            catch (Exception ex)
            {
                ViewData["Message"] = ex.Message;
                return View();
            }
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            try
            {
                var article = _context.Articles.FirstOrDefault(x => x.Id == id);

                if (article == null)
                {
                    return NotFound();
                }
                return View(article);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Article article)
        {
            try
            {
                var deletedArticle = _context.Articles.FirstOrDefault(x => x.Id == article.Id);
                if (deletedArticle == null)
                {
                    return View(deletedArticle);
                }
                deletedArticle.IsDeleted = true;
                deletedArticle.DeletedDate = DateTime.Now;
                var userId =_contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                article.UserId = userId;
                var user = await _userManager.FindByIdAsync(userId);
                deletedArticle.DeletedBy = user.UserName;
                _context.Articles.Update(deletedArticle);
                _context.SaveChanges();
                return Redirect("/admin/article");
            }
            catch (Exception)
            {
                return View();
            }
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            try
            {
                var article = _context.Articles
                    .Include(x => x.Category)
                    .Include(x => x.ArticleTags)
                    .ThenInclude(x => x.Tag)
                    .FirstOrDefault(x => x.Id == id);

                if (article == null)
                {
                    return NotFound();
                }
                var categories = _context.Categories.ToList();
                var tags = _context.Tags.ToList();
                ViewData["tags"] = tags;
                ViewData["categories"] = categories;
                return View(article);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Article article, List<int> tagIds, IFormFile Photo)
        {
            try
            {
                var categories = _context.Categories.ToList();
                var tags = _context.Tags.ToList();
                ViewData["tags"] = tags;
                ViewData["categories"] = categories;
                var updatedArticle = _context.Articles
                        .Include(x => x.Category)
                        .Include(x => x.ArticleTags)
                        .ThenInclude(x => x.Tag)
                        .FirstOrDefault(x => x.Id == article.Id);

                if (updatedArticle == null)
                {
                    return NotFound();
                }

                if (tagIds.Count == 0)
                {
                    ViewData["tagError"] = "Choose at least one tag";
                    return View(updatedArticle);
                }

                if (Photo != null)
                {
                    var photoUrl = await Photo.SaveFileAsync(_env.WebRootPath);
                    updatedArticle.PhotoUrl = photoUrl;
                }
                var userId = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var user = await _userManager.FindByIdAsync(userId);
                updatedArticle.Title = article.Title;
                updatedArticle.SeoUrl = SeoHelper.ConverToSeo(article.Title);
                updatedArticle.Content = article.Content;
                var checkUser = await _userManager.FindByIdAsync(updatedArticle.UserId);
                var userRoles = (await _userManager.GetRolesAsync(checkUser)).ToList();
                if (!updatedArticle.IsPublished && article.IsPublished)
                {
                    if (!userRoles.Contains("Author") && !userRoles.Contains("Admin") && !userRoles.Contains("Admin Editor"))
                    {
                        Notify newNotify1 = new()
                        {
                            CreatedDate = DateTime.Now,
                            Message = $"You're given <strong>Author</strong> role.",
                            Link = "/article/myArticles",
                            PhotoUrl = user.PhotoUrl,
                            UserId = checkUser.Id,
                            IsSeen = false,
                        };
                        await _userManager.AddToRoleAsync(checkUser, "Author");
                        _context.Notifies.Add(newNotify1);
                    }
                    Notify newNotify2 = new()
                    {
                        CreatedDate = DateTime.Now,
                        Message = $"Your article has been published now.",
                        Link = $"/article/{updatedArticle.SeoUrl}?id={updatedArticle.Id}",
                        PhotoUrl = updatedArticle.PhotoUrl,
                        UserId = updatedArticle.UserId,
                        IsSeen = false,
                    };
                    _context.Notifies.Add(newNotify2);
                }
                else if(updatedArticle.IsPublished && !article.IsPublished)
                {
                    Notify newNotify2 = new()
                    {
                        CreatedDate = DateTime.Now,
                        Message = $"Your \"{updatedArticle.Title}\" article has been unpublished now. See reason? Please contact us",
                        Link = "/contact",
                        PhotoUrl = updatedArticle.PhotoUrl,
                        UserId = updatedArticle.UserId,
                        IsSeen = false,
                    };
                    _context.Notifies.Add(newNotify2);
                }
                updatedArticle.IsPublished = article.IsPublished;
                updatedArticle.IsFeatured = article.IsFeatured;
                updatedArticle.IsInSlider = article.IsInSlider;
                updatedArticle.UpdatedDate = DateTime.Now;
                updatedArticle.CategoryId = article.CategoryId;
                updatedArticle.UpdatedBy = user.UserName;

                _context.Articles.Update(updatedArticle);
                _context.SaveChanges();
                if (article.ArticleTags!=null)
                {
                    article.ArticleTags.Clear();
                    for (int i = 0; i < tagIds.Count; i++)
                    {
                        ArticleTag tag = new ArticleTag()
                        {
                            TagId = tagIds[i],
                            ArticleId = article.Id,
                        };
                        article.ArticleTags.Add(tag);
                    }
                    _context.ArticleTags.UpdateRange(article.ArticleTags);
                    _context.SaveChanges();
                }
                return Redirect("/admin/article");
            }
            catch (Exception)
            {
                var categories = _context.Categories.ToList();
                var tags = _context.Tags.ToList();
                ViewData["tags"] = tags;
                ViewData["categories"] = categories;
                var updatedArticle = _context.Articles
                        .Include(x => x.Category)
                        .Include(x => x.ArticleTags)
                        .ThenInclude(x => x.Tag)
                        .FirstOrDefault(x => x.Id == article.Id);
                return View(updatedArticle);
            }
        }
    }
}
