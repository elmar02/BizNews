using BizNews.Models;

namespace BizNews.ViewModels
{
    public class HomeVM
    {
        public List<Article> ArticlesInSlider { get; set; }
        public List<Article> BreakingNews { get; set; }
        public List<Article> FeaturedNews { get; set; }
        public List<Article> LatestNews { get; set; }
        public List<Article> TrandingNews { get; set; }
        public Advert Advert { get; set; }
        public List<Tag> Tags { get; set; }
    }
}
