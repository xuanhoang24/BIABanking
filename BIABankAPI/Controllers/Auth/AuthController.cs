using BankingSystemAPI.Application.Dtos.Auth;
using BankingSystemAPI.Application.Services.Implementations.Admin;
using BankingSystemAPI.Application.Services.Interfaces.Customer;
using BankingSystemAPI.Domain.Entities.Users.Admin;
using BankingSystemAPI.Infrastructure.Security.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace BankingSystemAPI.Controllers.Auth
{
    [ApiController]
    [Route("api/auth")]
    [EnableRateLimiting("auth")]
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
            try
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
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            try
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
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            try
            {
                var success = await _customerService.VerifyEmailAsync(token);

                if (!success)
                    return BadRequest(new { error = "Invalid verification token" });

                return Ok(new { message = "Email verified successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
