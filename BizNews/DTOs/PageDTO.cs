namespace BizNews.DTOs
{
    public class PageDTO
    {
        public readonly int MaxArticleCount = 6;
        public int PageSize { get; set; }
        public int ActivePage { get; set; } = 1;
    }
}
