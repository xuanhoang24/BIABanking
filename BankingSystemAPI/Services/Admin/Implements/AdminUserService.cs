using BankingSystemAPI.DataLayer;
using BankingSystemAPI.Models.Users.Admin;
using Microsoft.EntityFrameworkCore;
using BankingSystemAPI.Security.Interfaces;

namespace BankingSystemAPI.Services.Admin.Implements
{
    public class AdminUserService
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public AdminUserService(AppDbContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task<AdminUser?> AuthenticateAsync(string email, string password)
        {
            email = email.ToLowerInvariant();

            var admin = await _context.AdminUsers
                .FirstOrDefaultAsync(a => a.Email == email && a.IsActive);

            if (admin == null)
                return null;

            if (!_passwordHasher.Verify(password, admin.PasswordHash, admin.PasswordSalt))
                return null;

            admin.LastLoginAt = DateTime.UtcNow;
            admin.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return admin;
        }
    }
}
