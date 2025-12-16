using BankingSystemMVC.Areas.Admin.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminOnly")]
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
            return View(transactions ?? new List<Models.TransactionListViewModel>());
        }
    }
}
