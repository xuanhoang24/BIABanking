using BankingSystemMVC.Areas.Admin.Models;
using BankingSystemMVC.Areas.Admin.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = PermissionCodes.CustomerRead)]
    public class CustomersController : Controller
    {
        private readonly IAdminCustomerApiClient _customerApi;

        public CustomersController(IAdminCustomerApiClient customerApi)
        {
            _customerApi = customerApi;
        }

        public async Task<IActionResult> Index()
        {
            var customers = await _customerApi.GetAllCustomersAsync();
            return View(customers ?? new List<Models.CustomerListViewModel>());
        }

        public async Task<IActionResult> Details(int id)
        {
            var customer = await _customerApi.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                TempData["Error"] = "Customer not found";
                return RedirectToAction(nameof(Index));
            }

            return View(customer);
        }

        public async Task<IActionResult> Accounts(int id)
        {
            var customer = await _customerApi.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                TempData["Error"] = "Customer not found";
                return RedirectToAction(nameof(Index));
            }

            var accounts = await _customerApi.GetCustomerAccountsAsync(id);
            ViewBag.Customer = customer;
            return View(accounts ?? new List<Models.AccountSummaryViewModel>());
        }

        public async Task<IActionResult> Transactions(int id)
        {
            var customer = await _customerApi.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                TempData["Error"] = "Customer not found";
                return RedirectToAction(nameof(Index));
            }

            var transactions = await _customerApi.GetCustomerTransactionsAsync(id);
            ViewBag.Customer = customer;
            return View(transactions ?? new List<Models.TransactionSummaryViewModel>());
        }

        [HttpPost]
        [Authorize(Policy = PermissionCodes.CustomerManage)]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var result = await _customerApi.UpdateCustomerStatusAsync(id, status);
            if (result)
            {
                TempData["Success"] = "Customer status updated successfully";
            }
            else
            {
                TempData["Error"] = "Failed to update customer status";
            }

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
