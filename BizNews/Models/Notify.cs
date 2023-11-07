namespace BizNews.Models
{
    public class Notify
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public bool IsSeen { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UserId { get; set; }
        public string Link { get; set; }
        public string PhotoUrl { get; set; }
    }
}
