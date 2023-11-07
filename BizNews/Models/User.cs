using Microsoft.AspNetCore.Identity;

namespace BizNews.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhotoUrl { get; set; }
        public List<Comment> Comments { get; set; }
        public List<HistoryRecord> HistoryRecords { get; set; }
    }
}
