namespace BizNews.Models
{
    public class HistoryRecord
    {
        public int Id { get; set; }
        public int ArticleId { get; set; }
        public DateTime SeenDate { get; set; }
        public Article Article { get; set; }
        public string UserId { get; set; }
    }
}
