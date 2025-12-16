using BankingSystemMVC.Models.Accounts;
using BankingSystemMVC.Models.Accounts.Transactions;
using BankingSystemMVC.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemMVC.Controllers.Accounts
{
    [Authorize]
    public class AccountsController : Controller
    {
        private readonly IAccountApiClient _accountApi;
        private readonly IAccountViewService _accountViewService;
        private readonly ICustomerApiClient _customerApi;

        public AccountsController(IAccountApiClient accountApi, IAccountViewService accountViewService, ICustomerApiClient customerApi)
        {
            _accountApi = accountApi;
            _accountViewService = accountViewService;
            _customerApi = customerApi;
        }

        // LIST ACCOUNTS
        // GET: /Accounts
        public async Task<IActionResult> Index()
        {
            var accounts = await _accountApi.GetMyAccountsAsync();
            return View(accounts);
        }

        // CREATE ACCOUNT
        // GET: /Accounts/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var customer = await _customerApi.GetMeAsync();
            
            if (customer == null || !customer.IsKYCVerified)
            {
                TempData["ErrorMessage"] = "You must complete KYC verification before creating an account.";
                TempData["ShowKycLink"] = true;
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAccountViewModel model)
        {
            // Double-check KYC verification
            var customer = await _customerApi.GetMeAsync();
            
            if (customer == null || !customer.IsKYCVerified)
            {
                TempData["ErrorMessage"] = "You must complete KYC verification before creating an account.";
                TempData["ShowKycLink"] = true;
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
                return View(model);

            var (success, errorMessage) = await _accountApi.CreateAccountAsync(model);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, errorMessage ?? "Failed to create account");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        // ACCOUNT DETAILS
        // GET: /Accounts/Details/{id}
        public async Task<IActionResult> Detail(int id)
        {
            var timeZoneId = Request.Cookies["timezone"];

            if (string.IsNullOrWhiteSpace(timeZoneId))
                timeZoneId = TimeZoneInfo.Local.Id;

            var account = await _accountViewService.GetAccountDetailAsync(id, timeZoneId);

            if (account == null)
                return RedirectToAction(nameof(Index));

            return View(account);
        }
    }
}
