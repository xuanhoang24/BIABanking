using BankingSystemMVC.Models.ViewModels.Auth;
using BankingSystemMVC.Services.Interfaces.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemMVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthApiClient _authApi;
        private readonly ILogger<AuthController> _logger;
        
        public AuthController(IAuthApiClient authApi, ILogger<AuthController> logger)
        {
            _authApi = authApi;
            _logger = logger;
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

            try
            {
                var result = await _authApi.LoginAsync(model);

                if (result == null)
                {
                    _logger.LogWarning("Login failed for email: {Email}", model.Email);
                    ModelState.AddModelError(string.Empty, "Invalid email or password");
                    return View(model);
                }

                _logger.LogInformation("Login successful for email: {Email}", model.Email);

                Response.Cookies.Append("customer_access_token", result.AccessToken,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = false, // localhost only
                        SameSite = SameSiteMode.Lax,
                        Path = "/",
                        Expires = DateTimeOffset.UtcNow.AddMinutes(15)
                    }
                );

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email: {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "An error occurred during login. Please try again.");
                return View(model);
            }
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
            Response.Cookies.Delete("customer_access_token", new CookieOptions
            {
                Path = "/",
                SameSite = SameSiteMode.Lax,
                Secure = false
            });

            return RedirectToAction("Login");
        }
    }
}
