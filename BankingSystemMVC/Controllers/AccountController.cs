using BankingSystemMVC.Models.Auth;
using BankingSystemMVC.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthApiClient _authApi;

        public AccountController(IAuthApiClient authApi)
        {
            _authApi = authApi;
        }

        // LOGIN

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _authApi.LoginAsync(model);

            if (result == null)
            {
                ModelState.AddModelError("", "Invalid email or password");
                return View(model);
            }

            Response.Cookies.Append(
                "access_token",
                result.AccessToken,
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
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var success = await _authApi.RegisterAsync(model);

            if (!success)
            {
                ModelState.AddModelError("", "Registration failed");
                return View(model);
            }

            return RedirectToAction("Login");
        }

        // LOGOUT

        public IActionResult Logout()
        {
            Response.Cookies.Delete("access_token");
            return RedirectToAction("Login");
        }
    }
}
