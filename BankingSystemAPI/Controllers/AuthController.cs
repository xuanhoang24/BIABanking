using BankingSystemAPI.Models.DTOs.Auth;
using BankingSystemAPI.Models.Users.Admin;
using BankingSystemAPI.Security.Interfaces;
using BankingSystemAPI.Services.Admin.Implements;
using BankingSystemAPI.Services.Customer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly AuditService _auditService;
        private readonly IJwtTokenService _jwt;

        public AuthController(ICustomerService customerService, AuditService auditService, IJwtTokenService jwt)
        {
            _customerService = customerService;
            _auditService = auditService;
            _jwt = jwt;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto request)
        {
            var customer = await _customerService.RegisterCustomerAsync(
                request.FirstName,
                request.LastName,
                request.Email,
                request.Password,
                request.PhoneNumber,
                request.DateOfBirth,
                request.Address
            );

            await _auditService.LogAsync(
                AuditAction.CustomerRegistration,
                "Customer",
                customer.Id,
                customer.Id,
                $"Customer registered with email {customer.Email}"
            );

            return Ok(new { customer.Id });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            var customer = await _customerService.AuthenticateCustomerAsync(
                request.Email,
                request.Password
            );

            if (customer == null)
                return Unauthorized();

            var token = _jwt.GenerateCustomerToken(customer);

            return Ok(new LoginResponseDto
            {
                AccessToken = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15)
            });
        }
    }
}
