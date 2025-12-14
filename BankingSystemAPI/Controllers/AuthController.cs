using BankingSystemAPI.Models.DTOs.Auth;
using BankingSystemAPI.Security.Interfaces;
using BankingSystemAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IJwtTokenService _jwt;

        public AuthController(UserService userService, IJwtTokenService jwt)
        {
            _userService = userService;
            _jwt = jwt;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto request)
        {
            var user = await _userService.RegisterUserAsync(
                request.FirstName,
                request.LastName,
                request.Email,
                request.Password,
                request.PhoneNumber,
                request.DateOfBirth,
                request.Address
            );

            return Ok(new { user.Id });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            var user = await _userService.AuthenticateUserAsync(
                request.Email,
                request.Password
            );

            if (user == null)
                return Unauthorized();

            var token = _jwt.GenerateUserToken(user);

            return Ok(new LoginResponseDto
            {
                AccessToken = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15)
            });
        }
    }
}
