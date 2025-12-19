using BankingSystemMVC.Areas.Admin.Models;
using BankingSystemMVC.Areas.Admin.Models.Constants;
using BankingSystemMVC.Areas.Admin.Services.Interfaces.Customers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemMVC.Areas.Admin.Controllers.Accounts
{
    [Area("Admin")]
    [Authorize(Policy = PermissionCodes.CustomerRead)]
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
            return View(accounts ?? new List<AccountListViewModel>());
        }
    }
}
