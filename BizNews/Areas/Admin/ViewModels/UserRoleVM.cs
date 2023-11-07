using BizNews.Models;

namespace BizNews.Areas.Admin.ViewModels
{
    public class UserRoleVM
    {
        public User User { get; set; }
        public List<string> Roles { get; set; }
    }
}
