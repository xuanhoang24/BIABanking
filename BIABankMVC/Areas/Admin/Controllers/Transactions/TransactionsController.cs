using BankingSystemMVC.Areas.Admin.Models;
using BankingSystemMVC.Areas.Admin.Models.Constants;
using BankingSystemMVC.Areas.Admin.Services.Interfaces.Customers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemMVC.Areas.Admin.Controllers.Transactions
{
    [Area("Admin")]
    [Authorize(Policy = PermissionCodes.TransactionRead)]
    public class TransactionsController : Controller
    {
        private readonly IAdminCustomerApiClient _customerApi;

        public TransactionsController(IAdminCustomerApiClient customerApi)
        {
            _customerApi = customerApi;
        }

        public async Task<IActionResult> Index([FromQuery] int limit = 100)
        {
            var transactions = await _customerApi.GetAllTransactionsAsync(limit);
            return View(transactions ?? new List<TransactionListViewModel>());
        }

        [HttpGet]
        public async Task<IActionResult> GetTransactionsList([FromQuery] int limit = 100)
        {
            var transactions = await _customerApi.GetAllTransactionsAsync(limit);
            return PartialView("_TransactionsList", transactions ?? new List<TransactionListViewModel>());
        }
    }
}
