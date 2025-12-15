using BankingSystemAPI.Models.Users;
using BankingSystemAPI.Models.Users.Admin;
using BankingSystemAPI.Security.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BankingSystemAPI.Security.Implements
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _config;

        public JwtTokenService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateCustomerToken(Customer customer)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, customer.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, customer.Email),
                new Claim("firstname", customer.FirstName)
            };

            return BuildToken(claims, 15);
        }

        public string GenerateAdminToken(AdminUser admin)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, admin.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, admin.Email),
                new Claim("is_admin", "true"),
                new Claim(ClaimTypes.Role, admin.Role.ToString())
            };

            return BuildToken(claims, 30);
        }

        private string BuildToken(Claim[] claims, int minutes)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"])
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(minutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
