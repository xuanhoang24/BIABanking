using BankingSystemAPI.Models.DTOs.Auth;
using BankingSystemAPI.Models.Users.Admin;
using BankingSystemAPI.Security.Interfaces;
using BankingSystemAPI.Services.Admin.Implements;
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

            await _auditService.LogAsync(
                AuditAction.AdminLogin,
                "AdminUser",
                admin.Id,
                null,
                $"Admin logged in: {admin.Email}"
            );

            var token = _jwt.GenerateAdminToken(admin);

            return Ok(new LoginResponseDto
            {
                AccessToken = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(30)
            });
        }
    }
}
