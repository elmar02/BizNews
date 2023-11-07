namespace BizNews.Models
{
    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string PhotoUrl { get; set; }
        public int ViewCount { get; set;}
        public string SeoUrl { get; set;}
        public DateTime CreatedDate { get; set;}
        public DateTime? UpdatedDate { get;set;}
        public DateTime? DeletedDate { get; set; }
        public bool IsPublished { get; set; }
        public bool IsDeleted { get; set;}
        public bool IsFeatured { get; set;}
        public bool IsInSlider { get; set;}
        public bool Ishidden { get; set;}
        public string CreatedBy { get; set;}
        public string? UpdatedBy { get; set;}
        public string? DeletedBy { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public int CategoryId { get; set;}
        public Category Category { get; set; }
        public List<ArticleTag> ArticleTags { get; set; }
        public List<Comment> Comments { get; set; }

    }
}
