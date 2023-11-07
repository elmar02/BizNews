using BizNews.DTOs;
using BizNews.Models;

namespace BizNews.ViewModels
{
    public class MyArticleVM
    {
        public List<Article> Articles { get; set; }
        public PageDTO PageDTO { get; set; }
    }
}
