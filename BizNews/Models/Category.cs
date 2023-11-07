namespace BizNews.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public string SeoUrl { get; set; }
        public List<Article> Articles { get; set; }
    }
}
