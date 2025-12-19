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
    }
}
