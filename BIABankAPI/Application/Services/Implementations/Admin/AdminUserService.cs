using BankingSystemAPI.Application.Dtos.Admin;
using BankingSystemAPI.Application.Services.Interfaces.Admin;
using BankingSystemAPI.Domain.Entities.Users.Admin;
using BankingSystemAPI.Domain.Entities.Users.Roles;
using BankingSystemAPI.Infrastructure.Persistence;
using BankingSystemAPI.Infrastructure.Security.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankingSystemAPI.Application.Services.Implementations.Admin
{
    public class AdminUserService : IAdminUserService
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

            // Check if account is locked
            if (admin.LockedUntil.HasValue && admin.LockedUntil.Value > DateTime.UtcNow)
            {
                var remainingMinutes = (int)(admin.LockedUntil.Value - DateTime.UtcNow).TotalMinutes;
                throw new InvalidOperationException($"Account is locked. Try again in {remainingMinutes} minutes.");
            }

            // Reset lock if expired
            if (admin.LockedUntil.HasValue && admin.LockedUntil.Value <= DateTime.UtcNow)
            {
                admin.LockedUntil = null;
                admin.FailedLoginAttempts = 0;
                await _context.SaveChangesAsync();
            }

            if (!_passwordHasher.Verify(password, admin.PasswordHash, admin.PasswordSalt))
            {
                // Increment failed login attempts
                admin.FailedLoginAttempts++;
                
                // Lock account after 5 failed attempts
                if (admin.FailedLoginAttempts >= 5)
                {
                    admin.LockedUntil = DateTime.UtcNow.AddMinutes(15);
                    await _context.SaveChangesAsync();
                    throw new InvalidOperationException("Account locked due to too many failed login attempts. Try again in 15 minutes.");
                }
                
                await _context.SaveChangesAsync();
                return null;
            }

            // Successful login - reset failed attempts
            if (admin.FailedLoginAttempts > 0)
            {
                admin.FailedLoginAttempts = 0;
            }

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
        public async Task<List<AdminUserListDto>> GetAllAdminUsersAsync()
        {
            var adminUsers = await _context.AdminUsers
                .Include(a => a.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => new AdminUserListDto
                {
                    Id = a.Id,
                    FirstName = a.FirstName,
                    LastName = a.LastName,
                    Email = a.Email,
                    Roles = a.UserRoles.Select(ur => ur.Role.Name).ToList(),
                    IsActive = a.IsActive,
                    LastLoginAt = a.LastLoginAt,
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync();

            return adminUsers;
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
                RequiresPasswordChange = true,
                UserRoles =
                {
                    new UserRole { RoleId = roleId }
                }
            };

            _context.AdminUsers.Add(adminUser);
            await _context.SaveChangesAsync();

            return adminUser;
        }

        public async Task<bool> UpdatePasswordAsync(string email, string newPassword)
        {
            email = email.ToLowerInvariant();

            var admin = await _context.AdminUsers
                .FirstOrDefaultAsync(a => a.Email == email && a.IsActive);

            if (admin == null)
                return false;

            _passwordHasher.CreateHash(newPassword, out var hash, out var salt);

            admin.PasswordHash = hash;
            admin.PasswordSalt = salt;
            admin.RequiresPasswordChange = false;
            admin.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<AdminUser?> GetAdminUserByIdAsync(int id)
        {
            return await _context.AdminUsers
                .Include(a => a.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<bool> ResetAdminPasswordAsync(int id)
        {
            var admin = await _context.AdminUsers.FindAsync(id);
            if (admin == null)
                return false;

            _passwordHasher.CreateHash("employee", out var hash, out var salt);

            admin.PasswordHash = hash;
            admin.PasswordSalt = salt;
            admin.RequiresPasswordChange = true;
            admin.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleAdminStatusAsync(int id)
        {
            var admin = await _context.AdminUsers.FindAsync(id);
            if (admin == null)
                return false;

            admin.IsActive = !admin.IsActive;
            admin.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
