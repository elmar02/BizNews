using System.ComponentModel.DataAnnotations;

namespace BizNews.DTOs
{
    public class RegisterDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public IFormFile? Photo { get; set; }
        public string Password { get; set; }
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        public bool RememberMe { get; set; }
    }
}
