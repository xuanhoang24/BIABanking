using BankingSystemMVC.Models.Constants.Auth;
using BankingSystemMVC.Models.ViewModels.Profile;
using BankingSystemMVC.Services.Interfaces.Customers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemMVC.Controllers.Customer
{
    [Authorize(Policy = CustomerPolicies.Authenticated)]
    public class ProfileController : Controller
    {
        private readonly ICustomerApiClient _customersApi;

        public ProfileController(ICustomerApiClient customersApi)
        {
            _customersApi = customersApi;
        }

        public async Task<IActionResult> Index()
        {
            var me = await _customersApi.GetMeAsync();
            if (me == null)
                return RedirectToAction("Login", "Auth");

            return View(me);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var success = await _customersApi.ChangePasswordAsync(model.CurrentPassword, model.NewPassword);

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
