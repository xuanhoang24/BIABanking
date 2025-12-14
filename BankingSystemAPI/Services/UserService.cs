using BankingSystemAPI.DataLayer;
using BankingSystemAPI.Models;
using BankingSystemAPI.Models.Users;
using BankingSystemAPI.Security.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankingSystemAPI.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;
        private readonly AuditService _auditService;
        private readonly IPasswordHasher _passwordHasher;

        public UserService( AppDbContext context, AuditService auditService, IPasswordHasher passwordHasher)
        {
            _context = context;
            _auditService = auditService;
            _passwordHasher = passwordHasher;
        }

        public async Task<User> RegisterUserAsync(
            string firstName,
            string lastName,
            string email,
            string password,
            string phoneNumber,
            DateTime dateOfBirth,
            string address)
        {
            email = email.ToLowerInvariant();

            if (await _context.Users.AnyAsync(u => u.Email == email))
                throw new InvalidOperationException("User with this email already exists");

            using var transaction = await _context.Database.BeginTransactionAsync();

            _passwordHasher.CreateHash(password, out var hash, out var salt);

            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PasswordHash = hash,
                PasswordSalt = salt,
                PhoneNumber = phoneNumber,
                DateOfBirth = dateOfBirth,
                Address = address
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await _auditService.LogAsync(
                AuditAction.UserRegistration,
                "User",
                user.Id,
                user.Id,
                $"User registered: {email}"
            );

            await transaction.CommitAsync();
            return user;
        }

        public async Task<User?> AuthenticateUserAsync(string email, string password)
        {
            email = email.ToLowerInvariant();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return null;

            if (!_passwordHasher.Verify(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }
    }
}
