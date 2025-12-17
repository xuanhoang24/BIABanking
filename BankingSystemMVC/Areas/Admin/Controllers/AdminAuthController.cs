using BankingSystemMVC.Areas.Admin.Models;
using BankingSystemMVC.Areas.Admin.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BankingSystemMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminAuthController : Controller
    {
        private readonly IAdminAuthApiClient _authApi;
        public AdminAuthController(IAdminAuthApiClient authApi)
        {
            _authApi = authApi;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AdminLoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _authApi.LoginAsync(model);

            if (result == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid admin credentials");
                return View(model);
            }

            Response.Cookies.Append(
                "admin_access_token",
                result.AccessToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Strict,
                    Path = "/Admin",
                    Expires = result.ExpiresAt
                }
            );
            var token = new JwtSecurityTokenHandler().ReadJwtToken(result.AccessToken);
            var hasReviewerRole = token.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "KycReviewer");

            if (hasReviewerRole)
                return RedirectToAction("Index", "AdminKyc", new { area = "Admin" });

            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("admin_access_token", 
                new CookieOptions
                {
                    Path = "/Admin"
                });

            return RedirectToAction("Login");
        }
    }
}
