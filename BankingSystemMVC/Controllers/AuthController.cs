using BankingSystemMVC.Models.Auth;
using BankingSystemMVC.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemMVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthApiClient _authApi;
        public AuthController(IAuthApiClient authApi)
        {
            _authApi = authApi;
        }

        // LOGIN
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            // If already logged in, redirect
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _authApi.LoginAsync(model);

            if (result == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password");
                return View(model);
            }

            Response.Cookies.Append("user_access_token", result.AccessToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false, // localhost only
                    SameSite = SameSiteMode.Lax,
                    Path = "/",
                    Expires = result.ExpiresAt
                }
            );

            return RedirectToAction("Index", "Home");
        }

        // REGISTER
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var success = await _authApi.RegisterAsync(model);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Registration failed");
                return View(model);
            }

            return RedirectToAction("Login");
        }

        // LOGOUT
        [Authorize]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("user_access_token", new CookieOptions
            {
                Path = "/"
            });

            return RedirectToAction("Login");
        }
    }
}
