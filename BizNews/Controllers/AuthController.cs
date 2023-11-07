using BizNews.DTOs;
using BizNews.Helper;
using BizNews.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BizNews.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IHttpContextAccessor _contextAccessor;

        private readonly IWebHostEnvironment _env;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, IWebHostEnvironment env, IHttpContextAccessor contextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _env = env;
            _contextAccessor = contextAccessor;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            try
            {
                var checkUser = await _userManager.FindByEmailAsync(loginDTO.EmailOrUsername);
                if (checkUser == null)
                {
                    checkUser = await _userManager.FindByNameAsync(loginDTO.EmailOrUsername);
                    if (checkUser == null)
                    {
                        ModelState.AddModelError("Error", "User not found!");
                        return View();
                    }
                }
                var result = await _signInManager.PasswordSignInAsync(checkUser.UserName, loginDTO.Password, loginDTO.RememberMe, true);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("Error", "Email/Username or password is incorrect!");
                    return View();
                }
                string returnUrl = Request.Query["ReturnUrl"].ToString();
                return Redirect(!string.IsNullOrEmpty(returnUrl) ? returnUrl : "/");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return View();
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            try
            {
                var checkUser = await _userManager.FindByEmailAsync(registerDTO.Email);
                if (checkUser != null)
                {
                    ModelState.AddModelError("Error", "Email is already used!");
                    checkUser = await _userManager.FindByNameAsync(registerDTO.UserName);
                    if (checkUser != null)
                    {
                        ModelState.AddModelError("Error", "Username is already used!");
                    }
                    return View();
                }
                checkUser = await _userManager.FindByNameAsync(registerDTO.UserName);
                if (checkUser != null)
                {
                    ModelState.AddModelError("Error", "Username is already used!");
                    return View();
                }
                var profilePhotoUrl = "/uploads/default.png";
                if (registerDTO.Photo != null)
                {
                    profilePhotoUrl = await registerDTO.Photo.SaveFileAsync(_env.WebRootPath);
                }
                User newUser = new()
                {
                    Email = registerDTO.Email,
                    UserName = registerDTO.UserName,
                    FirstName = registerDTO.FirstName,
                    LastName = registerDTO.LastName,
                    PhotoUrl = profilePhotoUrl,
                };
                var result = await _userManager.CreateAsync(newUser, registerDTO.Password);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("Error", error.Description);
                    }
                    return View();
                }
                await _signInManager.PasswordSignInAsync(registerDTO.UserName, registerDTO.Password, registerDTO.RememberMe, true);
                await _userManager.AddToRoleAsync(newUser, "User");
                return RedirectToAction("Index","Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error",ex.Message); 
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/auth/login");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Redirect("/auth/login");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string newPassword, string currentPassword)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return View();
                }
                var changePasswordResult = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
                if (!changePasswordResult.Succeeded)
                {
                    ModelState.AddModelError("Error", "Current password is incorrect!");
                    return View();
                }
                await _signInManager.SignOutAsync();
                return Redirect("/auth/login");
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return View();
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditProfile()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return View(currentUser);
        }
        [HttpPost]
        public async Task<IActionResult> EditProfile(User user,IFormFile Photo)
        {
            try
            {
                if (user == null)
                {
                    return NotFound();
                }
                if (Photo != null)
                {
                    user.PhotoUrl = await Photo.SaveFileAsync(_env.WebRootPath);
                }

                var  findUser = await _userManager.FindByIdAsync(user.Id);
                findUser.FirstName = user.FirstName;
                findUser.LastName = user.LastName;
                findUser.PhotoUrl = user.PhotoUrl;
                var result = await _userManager.UpdateAsync(findUser);
                if (!result.Succeeded)
                {
                    var currentUser = await _userManager.GetUserAsync(User);
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("Error", error.Description);
                    }
                    return View(currentUser);
                }
                string returnUrl = Request.Query["ReturnUrl"].ToString();
                return Redirect(!string.IsNullOrEmpty(returnUrl) ? returnUrl : "/");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                var currentUser = await _userManager.GetUserAsync(User);
                return View(currentUser);
            }
        }
    }
}
