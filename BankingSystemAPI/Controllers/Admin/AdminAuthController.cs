using BankingSystemAPI.Models.DTOs.Auth;
using BankingSystemAPI.Security.Interfaces;
using BankingSystemAPI.Services.Admin;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemAPI.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/auth")]
    public class AdminAuthController : ControllerBase
    {
        private readonly AdminUserService _adminService;
        private readonly IJwtTokenService _jwt;

        public AdminAuthController(AdminUserService adminService, IJwtTokenService jwt)
        {
            _adminService = adminService;
            _jwt = jwt;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(AdminLoginRequestDto dto)
        {
            var admin = await _adminService.AuthenticateAsync(dto.Email, dto.Password);

            if (admin == null)
                return Unauthorized();

            var token = _jwt.GenerateAdminToken(admin);

            return Ok(new LoginResponseDto
            {
                AccessToken = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(30)
            });
        }
    }
}
