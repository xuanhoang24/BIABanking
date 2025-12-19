using BankingSystemMVC.Areas.Admin.Models.Constants;
using BankingSystemMVC.Areas.Admin.Models.ViewModels.Auth;
using BankingSystemMVC.Areas.Admin.Models.ViewModels.Profile;
using BankingSystemMVC.Areas.Admin.Services.Interfaces.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankingSystemMVC.Areas.Admin.Controllers.Profile
{
    [Area("Admin")]
    [Authorize(Policy = PermissionCodes.DashboardView)]
    public class ProfileController : Controller
    {
        private readonly IAdminAuthApiClient _authApi;

        public ProfileController(IAdminAuthApiClient authApi)
        {
            _authApi = authApi;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new AdminProfileViewModel
            {
                FirstName = User.Identity?.Name ?? "Admin",
                Email = User.FindFirstValue(ClaimTypes.Email) ?? "",
                Roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel { IsFirstLogin = false });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var email = User.FindFirstValue(ClaimTypes.Email);
            
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError(string.Empty, "Unable to identify user. Please login again.");
                return View(model);
            }

            var success = await _authApi.ChangePasswordAsync(email, model.CurrentPassword, model.NewPassword);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Failed to change password. Please verify your current password.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Password changed successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
