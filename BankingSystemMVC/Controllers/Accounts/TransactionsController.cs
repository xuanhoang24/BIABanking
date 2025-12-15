using BankingSystemMVC.Models.Accounts.Transactions;
using BankingSystemMVC.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemMVC.Controllers.Accounts
{
    [Authorize]
    public class TransactionsController : Controller
    {
        private readonly ITransactionApiClient _transactionApi;
        private readonly IAccountApiClient _accountApi;
        public TransactionsController(ITransactionApiClient transactionApi, IAccountApiClient accountApiClient)
        {
            _transactionApi = transactionApi;
            _accountApi = accountApiClient;
        }

        // DEPOSIT
        [HttpGet]
        public IActionResult Deposit(int accountId)
        {
            return View(new DepositViewModel { AccountId = accountId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deposit(DepositViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (!await _transactionApi.DepositAsync(model))
            {
                ModelState.AddModelError("", "Deposit failed");
                return View(model);
            }

            return RedirectToAction("Index", "Accounts");
        }

        // WITHDRAW
        [HttpGet]
        public IActionResult Withdraw(int accountId)
        {
            return View(new WithdrawViewModel { AccountId = accountId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Withdraw(WithdrawViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (!await _transactionApi.WithdrawAsync(model))
            {
                ModelState.AddModelError("", "Withdraw failed");
                return View(model);
            }

            return RedirectToAction("Index", "Accounts");
        }

        // TRANSFER
        [HttpGet]
        public async Task<IActionResult> Transfer()
        {
            var accounts = await _accountApi.GetMyAccountsAsync();

            var vm = new TransferPageViewModel
            {
                Accounts = accounts
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Transfer(TransferPageViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Accounts = await _accountApi.GetMyAccountsAsync();
                return View(vm);
            }

            var model = vm.Transfer;
            var accounts = await _accountApi.GetMyAccountsAsync();
            vm.Accounts = accounts;

            // INTERNAL TRANSFER
            if (model.TransferType == "Internal")
            {
                if (!model.ToAccountId.HasValue)
                {
                    ModelState.AddModelError("", "Please select destination account");
                    vm.Accounts = await _accountApi.GetMyAccountsAsync();
                    return View(vm);
                }

                var toAccount = accounts
                    .FirstOrDefault(a => a.Id == model.ToAccountId.Value);

                if (toAccount == null)
                {
                    ModelState.AddModelError("", "Invalid destination account");
                    return View(vm);
                }

                model.ToAccountNumber = toAccount.AccountNumber;
            }

            // EXTERNAL TRANSFER
            if (model.TransferType == "External")
            {
                if (string.IsNullOrWhiteSpace(model.ToAccountNumber))
                {
                    ModelState.AddModelError("", "Please enter destination account number");
                    vm.Accounts = await _accountApi.GetMyAccountsAsync();
                    return View(vm);
                }
            }

            if (!await _transactionApi.TransferAsync(model))
            {
                ModelState.AddModelError("", "Transfer failed");
                vm.Accounts = await _accountApi.GetMyAccountsAsync();
                return View(vm);
            }

            return RedirectToAction("Index", "Accounts");
        }

        // PAYMENT
        [HttpGet]
        public IActionResult Payment(int accountId)
        {
            return View(new PaymentViewModel { AccountId = accountId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Payment(PaymentViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (!await _transactionApi.PaymentAsync(model))
            {
                ModelState.AddModelError("", "Payment failed");
                return View(model);
            }

            return RedirectToAction("Index", "Accounts");
        }
    }
}
