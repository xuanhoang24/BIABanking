using BankingSystemMVC.Areas.Admin.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminOnly")]
    public class AccountsController : Controller
    {
        private readonly IAdminCustomerApiClient _customerApi;

        public AccountsController(IAdminCustomerApiClient customerApi)
        {
            _customerApi = customerApi;
        }

        public async Task<IActionResult> Index()
        {
            var accounts = await _customerApi.GetAllAccountsAsync();
            return View(accounts ?? new List<Models.AccountListViewModel>());
        }
    }
}
