namespace BizNews.DTOs
{
    public class LoginDTO
    {
        public string EmailOrUsername { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
