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

        // ACCOUNT DETAILS (future)
        // GET: /Accounts/Details/{id}
        public IActionResult Details(int id)
        {
            // Placeholder
            return NotFound();
        }
    }
}
