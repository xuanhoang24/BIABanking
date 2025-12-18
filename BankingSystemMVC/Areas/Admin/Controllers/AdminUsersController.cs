using BankingSystemMVC.Areas.Admin.Models;
using BankingSystemMVC.Areas.Admin.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = PermissionCodes.CustomerManage)]
    public class AdminUsersController : Controller
    {
        private readonly IAdminUserApiClient _adminUserApiClient;

        public AdminUsersController(IAdminUserApiClient adminUserApiClient)
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