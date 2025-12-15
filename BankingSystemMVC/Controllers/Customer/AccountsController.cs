using BankingSystemMVC.Models.Accounts;
using BankingSystemMVC.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemMVC.Controllers.Customer
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

        // DEPOSIT FUNDS
        // GET: /Accounts/Deposit/{id}
        [Authorize]
        public IActionResult Deposit(int id)
        {
            return View(new DepositViewModel
            {
                AccountId = id
            });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deposit(DepositViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var amountInCents = (long)(model.Amount * 100);

            var success = await _accountApi.DepositAsync(
                model.AccountId,
                amountInCents,
                model.Description
            );

            if (!success)
            {
                ModelState.AddModelError("", "Deposit failed");
                return View(model);
            }

            return RedirectToAction("Index");
        }
    }
}
