using BankingSystemMVC.Areas.Admin.Models;
using BankingSystemMVC.Areas.Admin.Models.Constants;
using BankingSystemMVC.Areas.Admin.Models.ViewModels.Users;
using BankingSystemMVC.Areas.Admin.Services.Interfaces.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemMVC.Areas.Admin.Controllers.Users
{
    [Area("Admin")]
    [Authorize(Policy = PermissionCodes.CustomerManage)]
    public class UsersController : Controller
    {
        private readonly IAdminUserApiClient _adminUserApiClient;

        public UsersController(IAdminUserApiClient adminUserApiClient)
        {
            _adminUserApiClient = adminUserApiClient;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var adminUsers = await _adminUserApiClient.GetAdminUsersAsync();
            return View(adminUsers ?? new List<AdminUserListViewModel>());
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var user = await _adminUserApiClient.GetAdminUserByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Employee not found";
                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new AdminUserCreateViewModel();
            var roles = await _adminUserApiClient.GetRolesAsync();
            model.AvailableRoles = roles ?? new List<AdminRoleViewModel>();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminUserCreateViewModel model)
        {
            var roles = await _adminUserApiClient.GetRolesAsync();
            model.AvailableRoles = roles ?? new List<AdminRoleViewModel>();

            // Set default password
            model.Password = "employee";

            if (!ModelState.IsValid)
                return View(model);

            var success = await _adminUserApiClient.CreateAdminUserAsync(model);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Failed to create admin user. Please verify the details and try again.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Admin user created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(int id)
        {
            var success = await _adminUserApiClient.ResetAdminPasswordAsync(id);

            if (success)
            {
                TempData["SuccessMessage"] = "Employee password has been reset to 'employee'. They will be required to change it on next login.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to reset employee password";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var success = await _adminUserApiClient.ToggleAdminStatusAsync(id);

            if (success)
            {
                TempData["SuccessMessage"] = "Employee account status updated successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update employee account status";
            }

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}