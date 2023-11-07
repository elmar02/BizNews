using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BizNews.Controllers
{
    public class SearchController : Controller
    {
        [HttpPost]
        public IActionResult Search(string search)
        {
            string url = "/article" + (string.IsNullOrEmpty(search) ? "":$"?search={search}");
            return Redirect(url);
        }
    }
}
