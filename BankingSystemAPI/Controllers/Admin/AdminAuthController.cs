using BankingSystemAPI.Application.Dtos.Auth;
using BankingSystemAPI.Application.Services.Implementations.Admin;
using BankingSystemAPI.Infrastructure.Security.Interfaces;
using BankingSystemAPI.Domain.Entities.Users.Admin;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemAPI.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/auth")]
    public class AdminAuthController : ControllerBase
    {
        private readonly AdminUserService _adminService;
        private readonly AuditService _auditService;
        private readonly IJwtTokenService _jwt;

        public AdminAuthController(AdminUserService adminService, AuditService auditService, IJwtTokenService jwt)
        {
            _adminService = adminService;
            _auditService = auditService;
            _jwt = jwt;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(AdminLoginRequestDto dto)
        {
            var admin = await _adminService.AuthenticateAsync(dto.Email, dto.Password);

            if (admin == null)
                return Unauthorized();

            var token = await _jwt.GenerateAdminTokenAsync(admin);

            return Ok(new LoginResponseDto
            {
                AccessToken = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(30)
            });
        }
    }
}
