using BizNews.DTOs;
using BizNews.Models;

namespace BizNews.ViewModels
{
    public class ArticlesVM
    {
        public PageDTO PageDTO { get; set; }
        public List<Article> Articles { get; set; }
        public List<Article> TrandingNews { get; set; }
        public Advert Advert { get; set; }
        public List<Tag> Tags { get; set; }
        public string Search { get; set; }
    }
}
