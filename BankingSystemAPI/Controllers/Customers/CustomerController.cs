using BankingSystemAPI.Application.Services.Interfaces.Customer;
using BankingSystemAPI.Infrastructure.Persistence;
using BankingSystemAPI.Application.Dtos.Customers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BankingSystemAPI.Controllers.Customers
{
    [ApiController]
    [Route("api/customers")]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var customerIdClaim =
                User.FindFirst(ClaimTypes.NameIdentifier)
                ?? User.FindFirst("sub");

            if (customerIdClaim == null)
                return Unauthorized();

            var customerId = int.Parse(customerIdClaim.Value);

            var customer = await _customerService.GetMeAsync(customerId);

            if (customer == null)
                return NotFound();

            return Ok(customer);
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto dto)
        {
            var customerIdClaim =
                User.FindFirst(ClaimTypes.NameIdentifier)
                ?? User.FindFirst("sub");

            if (customerIdClaim == null)
                return Unauthorized();

            var customerId = int.Parse(customerIdClaim.Value);

            var success = await _customerService.ChangePasswordAsync(customerId, dto.CurrentPassword, dto.NewPassword);

            if (!success)
                return BadRequest(new { message = "Current password is incorrect or failed to update password" });

            return Ok(new { message = "Password changed successfully" });
        }
    }
}
