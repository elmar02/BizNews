using BizNews.Areas.Admin.ViewModels;
using BizNews.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BizNews.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    [Authorize(Roles = "Admin,Admin Editor")]
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddRole(string Id) 
        {
            if (Id == null) return NotFound();
            var checkUser = await _userManager.FindByIdAsync(Id);
            if (checkUser == null)
            {
                return NotFound();
            }
            var roles = _roleManager.Roles.ToList();
            var userRoles = (await _userManager.GetRolesAsync(checkUser)).ToList();
            var otherRoles = roles.Select(x=>x.Name).Except(userRoles).ToList();
            UserRoleVM roleVM = new UserRoleVM()
            {
                User = checkUser,
                Roles = otherRoles
            };
            return View(roleVM);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddRole(string userId, string role)
        {
            if (userId == null) return NotFound();
            var checkUser = await _userManager.FindByIdAsync(userId);
            if (checkUser == null)
            {
                return NotFound();
            }
            if (role == null)
            {
                return NotFound();
            }
            var result = await _userManager.AddToRoleAsync(checkUser, role);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("Error", "Something went wrong!");
                return View();
            }
            return Redirect("/admin/user");
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRole(string Id)
        {
            if (Id == null) return NotFound();
            var checkUser = await _userManager.FindByIdAsync(Id);
            if (checkUser == null)
            {
                return NotFound();
            }
            var userRoles = (await _userManager.GetRolesAsync(checkUser)).Where(x=>x!="User").ToList();
            UserRoleVM roleVM = new UserRoleVM()
            {
                User = checkUser,
                Roles = userRoles
            };
            return View(roleVM);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRole(string userId, string role)
        {
            if (userId == null) return NotFound();
            var checkUser = await _userManager.FindByIdAsync(userId);
            if (checkUser == null)
            {
                return NotFound();
            }
            if (role == null)
            {
                return NotFound();
            }
            var result = await _userManager.RemoveFromRoleAsync(checkUser, role);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("Error", "Something went wrong!");
                return View();
            }
            return Redirect("/admin/user");
        }
    }
}
