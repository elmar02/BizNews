using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BizNews.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    [Authorize(Roles = "Admin,Admin Editor")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(IdentityRole role) {
            try
            {
                var checkRole = await _roleManager.FindByNameAsync(role.Name);
                if (checkRole != null)
                {
                    ModelState.AddModelError("Error", "Role is already exist");
                    return View();
                }
                await _roleManager.CreateAsync(role);
                return Redirect("/admin/role");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return View();
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var checkRole = await _roleManager.FindByIdAsync(id);
            if (checkRole == null)
            {
                return NotFound();
            }
            return View(checkRole);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(IdentityRole role)
        {
            try
            {
                await _roleManager.DeleteAsync(role);
                return Redirect("/admin/role");
            }
            catch (Exception)
            {
                return Redirect("/admin/role");
            }
        }
    }
}
