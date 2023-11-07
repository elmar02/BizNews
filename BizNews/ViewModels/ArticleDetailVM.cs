using BizNews.Models;

namespace BizNews.ViewModels
{
    public class ArticleDetailVM
    {
        public Article Article { get; set; }
        public List<Article> BreakingNews { get; set; }
        public List<Article> SimilarArticles { get; set; }
        public Advert Advert { get; set; }
        public Article PreArticle { get; set; }
        public Article PostArticle { get; set; }
        public string CurrentUserId { get; set; }
    }
}
