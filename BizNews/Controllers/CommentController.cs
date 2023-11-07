using BizNews.Data;
using BizNews.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace BizNews.Controllers
{
    public class CommentController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<User> _userManager;
        public CommentController(AppDbContext context, IHttpContextAccessor contextAccessor, UserManager<User> userManager)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int id, string comment)
        {
            var article = _context.Articles
                .Where(x => x.IsPublished == true)
                .Where(x => x.IsDeleted == false)
                .Where(x => x.Ishidden == false)
                .Include(x => x.Comments)
                .FirstOrDefault(x => x.Id == id);
            if (article == null)
            {
                return NotFound();
            }
            if (comment.IsNullOrEmpty())
            {
                return Redirect($"/article/{article.SeoUrl}?id={article.Id}");
            }
            var userId = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await _userManager.FindByIdAsync(userId);
            Comment newComment = new()
            {
                ArticleId = article.Id,
                Content = comment,
                UserId = userId,
                CreatedDate = DateTime.Now,
            };
            article.Comments.Add(newComment);
            _context.Articles.Update(article);
            if (userId != article.UserId)
            {
                Notify newNotify = new()
                {
                    CreatedDate = DateTime.Now,
                    Message = $"<strong>{user.UserName}</strong> commented on your article.",
                    Link = $"/article/{article.SeoUrl}?id={article.Id}",
                    PhotoUrl = user.PhotoUrl,
                    UserId = article.UserId,
                    IsSeen = false,
                };
                _context.Notifies.Add(newNotify);
            }
            _context.SaveChanges();
            return Redirect($"/article/{article.SeoUrl}?id={article.Id}");

        }

        [HttpPost]
        public async Task<IActionResult> AddReply(int id, string reply)
        {
            var comment = _context.Comments
                .Include(x=>x.Replies)
                .Include(x=>x.ParentComment)
                .FirstOrDefault(x => x.Id == id);

            if (comment == null)
            {
                return NotFound();
            }

            var userId = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await _userManager.FindByIdAsync(userId);

            Comment newReply = new()
            {
                ArticleId = comment.ArticleId,
                ParentCommentId = comment.Id,
                Content = reply,
                UserId = userId,
                CreatedDate = DateTime.Now,
            };
            var article = _context.Articles
            .Where(x => x.IsPublished == true)
            .Where(x => x.IsDeleted == false)
            .Where(x => x.Ishidden == false)
            .Include(x => x.Comments)
            .FirstOrDefault(x => x.Id == comment.ArticleId);

            comment.Replies.Add(newReply);
            _context.Comments.Update(comment);
            _context.SaveChanges();

            var updatedComment = _context.Comments
                .Where(x => x.ParentCommentId != null)
                .Include(x => x.ParentComment)
                .FirstOrDefault(x => x.UserId == userId);
            if (userId != updatedComment.ParentComment.UserId)
            {
                Notify newNotify = new()
                {
                    CreatedDate = DateTime.Now,
                    Message = $"<strong>{user.UserName}</strong> replied: '{reply}'.",
                    Link = $"/article/{article.SeoUrl}?id={article.Id}",
                    PhotoUrl = user.PhotoUrl,
                    UserId = updatedComment.ParentComment.UserId,
                    IsSeen = false,
                };
                _context.Notifies.Add(newNotify);
            }
            _context.SaveChanges();
            return Redirect($"/article/{article.SeoUrl}?id={article.Id}");
        }

        [HttpPost]
        public IActionResult Delete(int id,int articleId)
        {
            var comment = _context.Comments.FirstOrDefault(x => x.Id == id);
            if (comment == null)
            {
                return NotFound();
            }
            var userId = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var article = _context.Articles
                .Where(x => x.IsPublished == true)
                .Where(x => x.IsDeleted == false)
                .Where(x => x.Ishidden == false)
                .FirstOrDefault(x => x.Id == articleId);

            if (!(comment.UserId == userId || article.UserId == userId))
            {
                return Redirect($"/article/{article.SeoUrl}?id={articleId}");
            }
            _context.Comments.Remove(comment);
            _context.SaveChanges();
            return Redirect($"/article/{article.SeoUrl}?id={articleId}");
        }
    }
}
