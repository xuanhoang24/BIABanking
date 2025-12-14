using BankingSystemMVC.Models.Accounts;
using BankingSystemMVC.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemMVC.Controllers.User
{
    [Authorize]
    public class AccountsController : Controller
    {
        private readonly IAccountApiClient _accountApi;

        public AccountsController(IAccountApiClient accountApi)
        {
            _accountApi = accountApi;
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
            var account = await _accountApi.GetAccountDetailAsync(id);

            if (account == null)
                return RedirectToAction(nameof(Index));

            return View(account);
        }
    }
}
