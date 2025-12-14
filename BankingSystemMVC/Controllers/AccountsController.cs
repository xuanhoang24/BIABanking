using BankingSystemMVC.Models.Accounts;
using BankingSystemMVC.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemMVC.Controllers
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
            var token = Request.Cookies["access_token"];
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            var accounts = await _accountApi.GetMyAccountsAsync(token);

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

            var token = Request.Cookies["access_token"];
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");

            var success = await _accountApi.CreateAccountAsync(token, model);

            if (!success)
            {
                ModelState.AddModelError("", "Failed to create account");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        // ACCOUNT DETAILS
        // GET: /Accounts/Details/{id}
        public async Task<IActionResult> Detail(int id)
        {
            var token = Request.Cookies["access_token"];
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            var account = await _accountApi.GetAccountDetailAsync(id, token);

            if (account == null)
                return RedirectToAction("Index");

            return View(account);
        }
    }
}
