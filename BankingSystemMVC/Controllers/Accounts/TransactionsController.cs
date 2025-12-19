using BankingSystemMVC.Models.Constants.Auth;
using BankingSystemMVC.Models.ViewModels.Accounts.Transactions;
using BankingSystemMVC.Services.Interfaces.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemMVC.Controllers.Accounts
{
    [Authorize(Policy = CustomerPolicies.Authenticated)]
    public class TransactionsController : Controller
    {
        private readonly ITransactionApiClient _transactionApi;
        private readonly IAccountApiClient _accountApi;

        public TransactionsController(
            ITransactionApiClient transactionApi,
            IAccountApiClient accountApi)
        {
            _transactionApi = transactionApi;
            _accountApi = accountApi;
        }

        [HttpGet]
        public IActionResult Confirm(TransactionConfirmViewModel vm)
        {
            return View(vm);
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

            var reference = await _transactionApi.DepositAsync(model);

            var account = (await _accountApi.GetMyAccountsAsync())
                .First(a => a.Id == model.AccountId);

            return RedirectToAction("Confirm", new TransactionConfirmViewModel
            {
                Title = "Deposit Confirmation",
                Type = "Deposit",
                ToAccountNumber = account.AccountNumber,
                Amount = model.Amount,
                Reference = reference
            });
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

            var reference = await _transactionApi.WithdrawAsync(model);

            var account = (await _accountApi.GetMyAccountsAsync())
                .First(a => a.Id == model.AccountId);

            return RedirectToAction("Confirm", new TransactionConfirmViewModel
            {
                Title = "Withdraw Confirmation",
                Type = "Withdraw",
                FromAccountNumber = account.AccountNumber,
                Amount = model.Amount,
                Reference = reference
            });
        }

        // TRANSFER
        [HttpGet]
        public async Task<IActionResult> Transfer()
        {
            return View(new TransferPageViewModel
            {
                Accounts = await _accountApi.GetMyAccountsAsync()
            });
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

            var accounts = await _accountApi.GetMyAccountsAsync();
            vm.Accounts = accounts;

            var fromAccount = accounts.First(a => a.Id == vm.Transfer.FromAccountId);

            if (vm.Transfer.TransferType == "Internal")
            {
                if (!vm.Transfer.ToAccountId.HasValue)
                {
                    ModelState.AddModelError("", "Please select destination account");
                    return View(vm);
                }

                var toAccountInternal = accounts
                    .First(a => a.Id == vm.Transfer.ToAccountId.Value);

                vm.Transfer.ToAccountNumber = toAccountInternal.AccountNumber;
            }

            var reference = await _transactionApi.TransferAsync(vm.Transfer);

            var toAccountNumber = vm.Transfer.ToAccountNumber;
            
            if (vm.Transfer.TransferType == "Internal" && vm.Transfer.ToAccountId.HasValue)
            {
                var toAccountInternal = accounts.FirstOrDefault(a => a.Id == vm.Transfer.ToAccountId.Value);
                if (toAccountInternal != null)
                {
                    toAccountNumber = toAccountInternal.AccountNumber;
                }
            }

            return RedirectToAction("Confirm", new TransactionConfirmViewModel
            {
                Title = "Transfer Confirmation",
                Type = "Transfer",
                FromAccountNumber = fromAccount.AccountNumber,
                ToAccountNumber = toAccountNumber,
                Amount = vm.Transfer.Amount,
                Reference = reference
            });
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

            var reference = await _transactionApi.PaymentAsync(model);

            var account = (await _accountApi.GetMyAccountsAsync())
                .First(a => a.Id == model.AccountId);

            return RedirectToAction("Confirm", new TransactionConfirmViewModel
            {
                Title = "Payment Confirmation",
                Type = "Payment",
                FromAccountNumber = account.AccountNumber,
                Amount = model.Amount,
                Reference = reference
            });
        }
    }
}
