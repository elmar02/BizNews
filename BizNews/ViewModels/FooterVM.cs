using BizNews.Models;

namespace BizNews.ViewModels
{
    public class FooterVM
    {
        public List<Category> Categories { get; set; }
        public List<Article> PopularNews { get; set; }
    }
}
