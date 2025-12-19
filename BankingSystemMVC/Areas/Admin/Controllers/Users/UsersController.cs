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
    }
}