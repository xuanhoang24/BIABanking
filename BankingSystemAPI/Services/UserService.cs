using BankingSystemAPI.DataLayer;
using BankingSystemAPI.Models;
using BankingSystemAPI.Models.Security;
using BankingSystemAPI.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace BankingSystemAPI.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;
        private readonly AuditService _auditService;
        private readonly PasswordOptions _options;

        public UserService( AppDbContext context, AuditService auditService, IOptions<PasswordOptions> options)
        {
            _context = context;
            _auditService = auditService;
            _options = options.Value;
        }

        public async Task<User> RegisterUserAsync( string firstName, string lastName, string email, string password,
            string phoneNumber, DateTime dateOfBirth, string address)
        {
            email = email.ToLowerInvariant();

            if (await _context.Users.AnyAsync(u => u.Email == email))
                throw new InvalidOperationException("User with this email already exists");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                CreatePasswordHash(password, out var hash, out var salt);

                var user = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    PasswordHash = hash,
                    PasswordSalt = salt,
                    PhoneNumber = phoneNumber,
                    DateOfBirth = dateOfBirth,
                    Address = address,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                await _auditService.LogAsync(
                    AuditAction.UserRegistration, "User", user.Id, user.Id, $"User registered: {email}");

                await transaction.CommitAsync();
                return user;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<User?> AuthenticateUserAsync(string email, string password)
        {
            email = email.ToLowerInvariant();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return null;

            if (user.LockedUntil.HasValue && user.LockedUntil > DateTime.UtcNow)
            {
                await _auditService.LogSuspiciousActivityAsync(
                    user.Id,
                    "Login attempt on locked account"
                );
                return null;
            }

            if (!VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
            {
                user.FailedLoginAttempts++;

                if (user.FailedLoginAttempts >= _options.MaxFailedAttempts)
                {
                    user.LockedUntil = DateTime.UtcNow.AddMinutes(_options.LockMinutes);

                    await _auditService.LogSuspiciousActivityAsync(
                        user.Id,
                        "Account locked due to multiple failed login attempts"
                    );
                }

                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return null;
            }

            user.FailedLoginAttempts = 0;
            user.LockedUntil = null;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await _auditService.LogAsync(
                AuditAction.UserLogin, "User", user.Id, user.Id, $"User logged in: {email}");

            return user;
        }

        private void CreatePasswordHash(string password, out string hash, out string salt)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                _options.SaltSize,
                _options.Iterations,
                HashAlgorithmName.SHA256
            );

            salt = Convert.ToBase64String(pbkdf2.Salt);
            hash = Convert.ToBase64String(pbkdf2.GetBytes(_options.HashSize));
        }

        private bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            var saltBytes = Convert.FromBase64String(storedSalt);

            using var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                saltBytes,
                _options.Iterations,
                HashAlgorithmName.SHA256
            );

            var computedHash = Convert.ToBase64String(
                pbkdf2.GetBytes(_options.HashSize)
            );

            return computedHash == storedHash;
        }
    }
}
