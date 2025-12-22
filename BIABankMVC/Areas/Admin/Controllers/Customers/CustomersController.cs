using BankingSystemMVC.Areas.Admin.Models;
using BankingSystemMVC.Areas.Admin.Models.Constants;
using BankingSystemMVC.Areas.Admin.Services.Interfaces.Customers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemMVC.Areas.Admin.Controllers.Customers
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
            return View(customers ?? new List<CustomerListViewModel>());
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
            return View(accounts ?? new List<AccountSummaryViewModel>());
        }

        [Authorize(Policy = PermissionCodes.TransactionRead)]
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
            return View(transactions ?? new List<TransactionSummaryViewModel>());
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

        [HttpPost]
        [Authorize(Policy = PermissionCodes.CustomerManage)]
        public async Task<IActionResult> ResetPassword(int id)
        {
            var result = await _customerApi.ResetCustomerPasswordAsync(id);
            if (result)
            {
                TempData["Success"] = "Customer password has been reset successfully. The new password is based on their first name and date of birth.";
            }
            else
            {
                TempData["Error"] = "Failed to reset customer password";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [Authorize(Policy = PermissionCodes.CustomerManage)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _customerApi.DeleteCustomerAsync(id);
            if (result)
            {
                TempData["Success"] = "Customer has been deleted successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["Error"] = "Failed to delete customer";
                return RedirectToAction(nameof(Details), new { id });
            }
        }
    }
}
