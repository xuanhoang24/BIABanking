using BankingSystemAPI.Domain.Entities.Users.Admin;
using BankingSystemAPI.Domain.Entities.Users.Customers;
using BankingSystemAPI.Infrastructure.Security.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BankingSystemAPI.Infrastructure.Security.Implements
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _config;
        private readonly IPermissionService _permissionService;

        public JwtTokenService(IConfiguration config, IPermissionService permissionService)
        {
            _config = config;
            _permissionService = permissionService;
        }

        public string GenerateCustomerToken(Customer customer)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, customer.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, customer.Email),
                new Claim("firstname", customer.FirstName),
                new Claim("kyc_verified", customer.IsKYCVerified ? "true" : "false")
            };

            return BuildToken(claims, 15);
        }

        public async Task<string> GenerateAdminTokenAsync(AdminUser admin)
        {
            var permissions = await _permissionService.GetPermissionsAsync(admin.Id);
            var roles = admin.UserRoles
                .Select(ur => ur.Role)
                .Where(r => r != null)
                .Select(r => r!.Name)
                .Distinct()
                .ToList();

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, admin.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString()),
                new Claim(ClaimTypes.Name, admin.FirstName),
                new Claim(JwtRegisteredClaimNames.Email, admin.Email),
                new Claim("firstname", admin.FirstName),
                new Claim("lastname", admin.LastName)
            };

            claims.AddRange(permissions.Select(p => new Claim("perm", p)));
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            return BuildToken(claims.ToArray(), 30);
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
