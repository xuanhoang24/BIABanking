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

        public AccountsController(IAccountApiClient accountApi, IAccountViewService accountViewService)
        {
            _accountApi = accountApi;
            _accountViewService = accountViewService;
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
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAccountViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var success = await _accountApi.CreateAccountAsync(model);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Failed to create account");
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
