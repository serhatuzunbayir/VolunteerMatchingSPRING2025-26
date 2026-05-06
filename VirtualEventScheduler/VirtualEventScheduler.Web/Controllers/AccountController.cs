using Microsoft.AspNetCore.Mvc;
using VirtualEventScheduler.Web.Models;
using VirtualEventScheduler.Web.Services;

namespace VirtualEventScheduler.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApiService _apiService;

        public AccountController(ApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var response = await _apiService.RegisterAsync(model);
                
                HttpContext.Session.SetString("Token", response.Token);
                HttpContext.Session.SetString("UserName", response.FullName);
                HttpContext.Session.SetString("UserEmail", response.Email);
                HttpContext.Session.SetString("UserRole", response.Role);

                TempData["Success"] = "Registration successful! Welcome to our platform.";
                return RedirectToAction("Index", "Events");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var response = await _apiService.LoginAsync(model);
                
                HttpContext.Session.SetString("Token", response.Token);
                HttpContext.Session.SetString("UserName", response.FullName);
                HttpContext.Session.SetString("UserEmail", response.Email);
                HttpContext.Session.SetString("UserRole", response.Role);

                TempData["Success"] = $"Welcome back, {response.FullName}!";
                return RedirectToAction("Index", "Events");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Invalid email or password");
                return View(model);
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Success"] = "You have been logged out.";
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Admin access required";
                return RedirectToAction("Index", "Events");
            }

            try
            {
                var users = await _apiService.GetUsersAsync();
                return View(users);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Could not load users: {ex.Message}";
                return View(new List<UserViewModel>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRole(int id, string role)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Admin access required";
                return RedirectToAction("Index", "Events");
            }

            try
            {
                var updated = await _apiService.UpdateUserRoleAsync(id, role);
                TempData["Success"] = $"Role of {updated.FullName} updated to {updated.Role}";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Users));
        }

        private bool IsAdmin() =>
            !string.IsNullOrEmpty(HttpContext.Session.GetString("Token")) &&
            HttpContext.Session.GetString("UserRole") == "Admin";
    }
}
