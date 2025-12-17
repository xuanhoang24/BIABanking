using BankingSystemAPI.DataLayer;
using BankingSystemAPI.Models.Users.Admin;
using BankingSystemAPI.Models.Users.Roles;
using BankingSystemAPI.Security.Interfaces;
using Microsoft.EntityFrameworkCore;

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
                .Include(a => a.UserRoles)
                    .ThenInclude(ur => ur.Role)
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

        public async Task<IReadOnlyList<Role>> GetRolesAsync()
        {
            return await _context.Roles
                .AsNoTracking()
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        public async Task<AdminUser?> CreateAdminUserAsync(string firstName, string lastName, string email, string password, int roleId)
        {
            email = email.ToLowerInvariant();

            var roleExists = await _context.Roles.AnyAsync(r => r.Id == roleId);
            if (!roleExists)
                return null;

            var emailExists = await _context.AdminUsers.AnyAsync(u => u.Email == email);
            if (emailExists)
                return null;

            _passwordHasher.CreateHash(password, out var hash, out var salt);

            var adminUser = new AdminUser
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PasswordHash = hash,
                PasswordSalt = salt,
                IsActive = true,
                UserRoles =
                {
                    new UserRole { RoleId = roleId }
                }
            };

            _context.AdminUsers.Add(adminUser);
            await _context.SaveChangesAsync();

            return adminUser;
        }
    }
}
