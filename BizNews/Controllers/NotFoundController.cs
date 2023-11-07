using Microsoft.AspNetCore.Mvc;

namespace BizNews.Controllers
{
    public class NotFoundController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
