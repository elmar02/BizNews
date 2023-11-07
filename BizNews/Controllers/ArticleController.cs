using BizNews.Data;
using BizNews.DTOs;
using BizNews.Helper;
using BizNews.Models;
using BizNews.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BizNews.Controllers
{
    [Route("article")]
    public class ArticleController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<User> _userManager;
        public ArticleController(AppDbContext context, IHttpContextAccessor contextAccessor, IWebHostEnvironment env, UserManager<User> userManager)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            _env = env;
            _userManager = userManager;
        }

        public IActionResult Index(string search, int page)

        {
            var articles = _context.Articles
                .Include(x => x.Category)
                .Include(x => x.ArticleTags)
                .ThenInclude(x => x.Tag)
                .Include(x => x.User)
                .Include(x => x.Comments)
                .OrderByDescending(x => x.CreatedDate)
                .Where(x => x.IsPublished == true)
                .Where(x => x.IsDeleted == false)
                .Where(x => x.Ishidden == false)
                .ToList();
            if (search != null)
            {
                search = search.ToLower();
                articles = articles.Where(x =>
                x.Content.ToLower().Contains(search) ||
                x.Category.CategoryName.ToLower().Contains(search) ||
                x.Category.SeoUrl.ToLower().Contains(search) ||
                x.Title.ToLower().Contains(search) ||
                x.SeoUrl.ToLower().Contains(search) ||
                x.ArticleTags.Select(y => y.Tag).Any(z => z.TagName.ToLower().Contains(search)) ||
                x.ArticleTags.Select(y => y.Tag).Any(z => z.SeoUrl.ToLower().Contains(search))
                ).ToList();
            }

            var tags = _context.Tags.ToList();

            ArticlesVM articlesVM = new()
            {
                Articles = articles,
                PageDTO = articles.CreatePageDTO(page),
                Tags = tags,
                Advert = _context.Adverts.FirstOrDefault(),
                TrandingNews = _context.Articles
                .Where(x => x.IsPublished == true)
                .Where(x => x.IsDeleted == false)
                .Where(x => x.Ishidden == false)
                .OrderByDescending(x => x.ViewCount).ThenByDescending(x => x.Comments.Count).Take(5).ToList(),
                Search = search,
            };
            return View(articlesVM);
        }

        [HttpGet("{seoUrl}")]
        public IActionResult Details(string seoUrl, int id)
        {
            var articles = _context.Articles
                .Include(x => x.Category)
                .Include(x => x.ArticleTags)
                .ThenInclude(x => x.Tag)
                .Where(x => x.IsPublished == true)
                .Where(x => x.IsDeleted == false)
                .Where(x => x.Ishidden == false)
                .OrderByDescending(x => x.CreatedDate)
                ;

            var article = articles
                .Include(x => x.Comments)
                .ThenInclude(x => x.Replies)
                .ThenInclude(x => x.User)
                .Include(x => x.Comments)
                .ThenInclude(x => x.User)
                .Include(x => x.User)
                .FirstOrDefault(x => x.Id == id);
            if (article == null)
            {
                return Redirect("/notFound");
            }
            if (article.SeoUrl != seoUrl)
            {
                return Redirect("/notFound");
            }


            var tags = article.ArticleTags.Select(x => x.Tag);
            var similarArticles = articles
            .Where(x =>
                (x.CategoryId == article.CategoryId ||
                x.ArticleTags.Any(at => tags.Contains(at.Tag))
                ) && x.Id != article.Id
            )
            .Take(5)
            .ToList();

            var index = articles.ToList().IndexOf(article);
            string? userId = null;

            if (User.Identity.IsAuthenticated)
            {
                userId = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            }

            ArticleDetailVM articleDetailVM = new()
            {
                Article = article,
                SimilarArticles = similarArticles,
                Advert = _context.Adverts.FirstOrDefault(),
                BreakingNews = articles.Take(2).ToList(),
                PreArticle = articles.ToList().ElementAtOrDefault(index - 1),
                PostArticle = articles.ToList().ElementAtOrDefault(index + 1),
                CurrentUserId = userId,
            };

            if (userId != null)
            {
                var record = _context.HistoryRecords.FirstOrDefault(x => x.UserId == userId && x.ArticleId == article.Id);
                if (record == null)
                {
                    HistoryRecord newRecord = new()
                    {
                        UserId = userId,
                        ArticleId = article.Id,
                        SeenDate = DateTime.Now,
                    };
                    article.ViewCount++;
                    _context.HistoryRecords.Add(newRecord);
                    _context.Articles.Update(article);
                    _context.SaveChanges();
                }
            }
            else
            {
                article.ViewCount++;
                _context.Articles.Update(article);
            }

            return View(articleDetailVM);
        }

        [Authorize]
        [HttpGet("create")]
        public IActionResult Create()
        {
            var categories = _context.Categories.ToList();
            var tags = _context.Tags.ToList();
            ViewData["tags"] = tags;
            ViewData["categories"] = categories;
            return View();
        }

        [Authorize]
        [HttpPost("create")]
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
                var userId = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                article.UserId = userId;
                var user = await _userManager.FindByIdAsync(userId);
                article.CreatedBy = user.UserName;
                article.ViewCount = 0;
                article.IsDeleted = false;
                article.IsPublished = false;
                article.IsInSlider = false;
                article.IsFeatured = false;
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
                return Redirect("/article/sent");
            }
            catch (Exception ex)
            {
                ViewData["Message"] = ex.Message;
                return View();
            }
        }

        [HttpGet("sent")]
        public IActionResult Sent()
        {
            return View();
        }

        [Authorize(Roles = "Admin, Admin Editor, Author")]
        [HttpGet("myArticles")]
        public IActionResult MyArticles(int page)
        {
            var userId = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var myArticles = _context.Articles
                .Include(x => x.Category)
                .Include(x => x.User)
                .Include(x => x.Comments)
                .OrderByDescending(x => x.CreatedDate)
                .Where(x => x.IsPublished == true)
                .Where(x => x.IsDeleted == false)
                .Where(x => x.UserId == userId)
                .ToList();
            MyArticleVM myArticleVM = new()
            {
                Articles = myArticles,
                PageDTO = myArticles.CreatePageDTO(page)
            };
            return View(myArticleVM);
        }

        [Authorize(Roles = "Admin, Admin Editor, Author")]
        [HttpGet("Edit")]
        public IActionResult Edit(int id)
        {
            var article = _context.Articles
                    .Include(x => x.Category)
                    .Include(x => x.ArticleTags)
                    .ThenInclude(x => x.Tag)
                    .Where(x => x.IsPublished == true)
                    .Where(x => x.IsDeleted == false)
                    .FirstOrDefault(x => x.Id == id);
            if (article == null) return NotFound();
            var userId = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (article.UserId != userId) return new ForbidResult();
            var categories = _context.Categories.ToList();
            var tags = _context.Tags.ToList();
            ViewData["tags"] = tags;
            ViewData["categories"] = categories;
            return View(article);
        }

        [Authorize(Roles = "Admin, Admin Editor, Author")]
        [HttpPost("Edit")]
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
                        .Where(x => x.IsPublished == true)
                        .Where(x => x.IsDeleted == false)
                        .FirstOrDefault(x => x.Id == article.Id);

                if (updatedArticle == null)
                {
                    return NotFound();
                }
                var userId = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                if (updatedArticle.UserId != userId) return new ForbidResult();
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

                updatedArticle.Title = article.Title;
                updatedArticle.Content = article.Content;
                updatedArticle.UpdatedDate = DateTime.Now;
                updatedArticle.CategoryId = article.CategoryId;
                var user = await _userManager.FindByIdAsync(userId);
                updatedArticle.UpdatedBy = user.UserName;
                updatedArticle.SeoUrl = SeoHelper.ConverToSeo(article.Title);
                _context.Articles.Update(updatedArticle);
                _context.SaveChanges();
                if (article.ArticleTags != null)
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
                return Redirect($"/article/{updatedArticle.SeoUrl}?id={updatedArticle.Id}");
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
                        .Where(x => x.IsPublished == true)
                        .Where(x => x.IsDeleted == false)
                        .FirstOrDefault(x => x.Id == article.Id);
                return View(updatedArticle);
            }
        }

        [Authorize(Roles = "Admin, Admin Editor, Author")]
        [HttpPost("HideToggle")]
        public IActionResult HideToggle(int id)
        {
            var article = _context.Articles.FirstOrDefault(x => x.Id == id);
            if (article == null)
            {
                return NotFound();
            }
            article.Ishidden = !article.Ishidden;
            _context.Articles.Update(article);
            _context.SaveChanges();
            return Redirect("/article/myArticles");
        }

        [Authorize(Roles = "Admin, Admin Editor, Author")]
        [HttpPost("Delete")]
        public IActionResult Delete(int id)
        {
            var article = _context.Articles.FirstOrDefault(x => x.Id == id);
            if (article == null)
            {
                return NotFound();
            }
            article.IsDeleted = true;
            _context.Articles.Update(article);
            _context.SaveChanges();
            return Redirect("/article/myArticles");
        }

    }
}