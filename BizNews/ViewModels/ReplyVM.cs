using BizNews.Models;

namespace BizNews.ViewModels
{
    public class ReplyVM
    {
        public List<Comment> Replies { get; set; }
        public string CurrentUserId { get; set; }
        public string UserId { get; set; }
    }
}
