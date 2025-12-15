using BankingSystemMVC.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemMVC.Controllers.Customer
{
    [Authorize]
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
    }
}
