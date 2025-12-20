using BankingSystemAPI.Application.Dtos.Admin;
using BankingSystemAPI.Application.Services.Interfaces.Admin;
using BankingSystemAPI.Domain.Entities.Users.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemAPI.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/customers")]
    [Authorize(Policy = PermissionCodes.CustomerRead)]
    public class AdminCustomersController : ControllerBase
    {
        private readonly ICustomerAdminService _customerService;
        private readonly IDashboardService _dashboardService;

        public AdminCustomersController(ICustomerAdminService customerService, IDashboardService dashboardService)
        {
            _customerService = customerService;
            _dashboardService = dashboardService;
        }

        [HttpGet("stats")]
        [Authorize(Policy = PermissionCodes.DashboardView)]
        public async Task<IActionResult> GetDashboardStats()
        {
            var stats = await _dashboardService.GetDashboardStatsAsync();
            return Ok(stats);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            return Ok(customers);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = PermissionCodes.CustomerManage)]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
                return NotFound(new { message = "Customer not found" });

            return Ok(customer);
        }

        [HttpGet("{id}/accounts")]
        public async Task<IActionResult> GetCustomerAccounts(int id)
        {
            var accounts = await _customerService.GetCustomerAccountsAsync(id);
            return Ok(accounts);
        }

        [HttpGet("{id}/transactions")]
        [Authorize(Policy = PermissionCodes.TransactionRead)]
        public async Task<IActionResult> GetCustomerTransactions(int id, [FromQuery] int limit = 50)
        {
            var transactions = await _customerService.GetCustomerTransactionsAsync(id, limit);
            return Ok(transactions);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateCustomerStatus(int id, [FromBody] UpdateStatusRequest request)
        {
            var result = await _customerService.UpdateCustomerStatusAsync(id, request.Status);
            if (!result)
                return BadRequest(new { message = "Failed to update customer status" });

            return Ok(new { message = "Customer status updated successfully" });
        }

        [HttpPost("{id}/reset-password")]
        [Authorize(Policy = PermissionCodes.CustomerManage)]
        public async Task<IActionResult> ResetCustomerPassword(int id)
        {
            var result = await _customerService.ResetCustomerPasswordAsync(id);
            if (!result)
                return BadRequest(new { message = "Failed to reset customer password" });

            return Ok(new { message = "Customer password reset successfully" });
        }

        [HttpGet("all-accounts")]
        public async Task<IActionResult> GetAllAccounts()
        {
            var accounts = await _customerService.GetAllAccountsAsync();
            return Ok(accounts);
        }

        [HttpGet("all-transactions")]
        [Authorize(Policy = PermissionCodes.TransactionRead)]
        public async Task<IActionResult> GetAllTransactions([FromQuery] int limit = 100)
        {
            var transactions = await _customerService.GetAllTransactionsAsync(limit);
            return Ok(transactions);
        }
    }
}
