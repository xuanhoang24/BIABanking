using BankingSystemAPI.Application.Dtos.Customers;
using BankingSystemAPI.Application.Services.Implementations.Admin;
using BankingSystemAPI.Application.Services.Interfaces.Customer;
using BankingSystemAPI.Application.Services.Interfaces.Email;
using BankingSystemAPI.Infrastructure.Persistence;
using BankingSystemAPI.Infrastructure.Security.Interfaces;
using BankingSystemAPI.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace BankingSystemAPI.Application.Services.Implementations.Customer
{
    public class CustomerService : ICustomerService
    {
        private readonly AppDbContext _context;
        private readonly AuditService _auditService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly NotificationService _notification;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public CustomerService(
            AppDbContext context, 
            AuditService auditService, 
            IPasswordHasher passwordHasher, 
            NotificationService notification,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _context = context;
            _auditService = auditService;
            _passwordHasher = passwordHasher;
            _notification = notification;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<Domain.Entities.Users.Customers.Customer> RegisterCustomerAsync(
            string firstName,
            string lastName,
            string email,
            string password,
            string phoneNumber,
            DateTime dateOfBirth,
            string address)
        {
            email = email.ToLowerInvariant();

            if (await _context.Customers.AnyAsync(c => c.Email == email))
                throw new InvalidOperationException("Customer with this email already exists");

            using var transaction = await _context.Database.BeginTransactionAsync();

            _passwordHasher.CreateHash(password, out var hash, out var salt);

            // Generate email verification token
            var verificationToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace("+", "-").Replace("/", "_").TrimEnd('=');

            var customer = new Domain.Entities.Users.Customers.Customer
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PasswordHash = hash,
                PasswordSalt = salt,
                PhoneNumber = phoneNumber,
                DateOfBirth = dateOfBirth,
                Address = address,
                EmailVerified = false,
                EmailVerificationToken = verificationToken,
                EmailVerificationTokenExpiry = DateTime.UtcNow.AddHours(24)
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            await _notification.NotifyAllAsync();
            
            // Send verification email
            var baseUrl = _configuration["AppSettings:BaseUrl"] ?? "http://localhost:7000";
            await _emailService.SendVerificationEmailAsync(email, firstName, verificationToken, baseUrl);
   
            
            return customer;
        }

        public async Task<Domain.Entities.Users.Customers.Customer?> AuthenticateCustomerAsync(string email, string password)
        {
            email = email.ToLowerInvariant();

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == email);
            if (customer == null)
                return null;

            // Check if account is locked
            if (customer.LockedUntil.HasValue && customer.LockedUntil.Value > DateTime.UtcNow)
            {
                var remainingMinutes = (int)(customer.LockedUntil.Value - DateTime.UtcNow).TotalMinutes;
                throw new InvalidOperationException($"Account is locked. Try again in {remainingMinutes} minutes.");
            }

            // Reset lock if expired
            if (customer.LockedUntil.HasValue && customer.LockedUntil.Value <= DateTime.UtcNow)
            {
                customer.LockedUntil = null;
                customer.FailedLoginAttempts = 0;
                await _context.SaveChangesAsync();
            }

            if (!customer.EmailVerified)
                throw new InvalidOperationException("Please verify your email before logging in");

            if (!_passwordHasher.Verify(password, customer.PasswordHash, customer.PasswordSalt))
            {
                // Increment failed login attempts
                customer.FailedLoginAttempts++;
                
                // Lock account after 5 failed attempts
                if (customer.FailedLoginAttempts >= 5)
                {
                    customer.LockedUntil = DateTime.UtcNow.AddMinutes(15);
                    await _context.SaveChangesAsync();
                    
                    await _auditService.LogAsync(
                        Domain.Entities.Users.Admin.AuditAction.CustomerRegistration,
                        "Customer",
                        customer.Id,
                        customer.Id,
                        $"Account locked due to {customer.FailedLoginAttempts} failed login attempts"
                    );
                    
                    throw new InvalidOperationException("Account locked due to too many failed login attempts. Try again in 15 minutes.");
                }
                
                await _context.SaveChangesAsync();
                return null;
            }

            // Successful login - reset failed attempts
            if (customer.FailedLoginAttempts > 0)
            {
                customer.FailedLoginAttempts = 0;
                await _context.SaveChangesAsync();
            }

            return customer;
        }

        public async Task<CustomerMeDto?> GetMeAsync(int customerId)
        {
            var customer = await _context.Customers
                .AsNoTracking()
                .Where(c => c.Id == customerId)
                .Select(c => new
                {
                    c.Id,
                    c.FirstName,
                    c.LastName,
                    c.Email,
                    c.IsKYCVerified
                })
                .FirstOrDefaultAsync();

            if (customer == null)
                return null;

            var latestKyc = await _context.KYCDocuments
                .AsNoTracking()
                .Where(k => k.CustomerId == customerId)
                .OrderByDescending(k => k.CreatedAt)
                .FirstOrDefaultAsync();

            return new CustomerMeDto
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                IsKYCVerified = customer.IsKYCVerified,

                HasKycSubmission = latestKyc != null,
                CurrentKycStatus = latestKyc?.Status
            };
        }

        public async Task<bool> ChangePasswordAsync(int customerId, string currentPassword, string newPassword)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == customerId);

            if (customer == null)
                return false;

            // Verify current password
            if (!_passwordHasher.Verify(currentPassword, customer.PasswordHash, customer.PasswordSalt))
                return false;

            // Update to new password
            _passwordHasher.CreateHash(newPassword, out var hash, out var salt);
            customer.PasswordHash = hash;
            customer.PasswordSalt = salt;
            customer.RequiresPasswordChange = false; // Clear the flag
            customer.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await _notification.NotifyAllAsync();
            return true;
        }

        public async Task<bool> VerifyEmailAsync(string token)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.EmailVerificationToken == token);

            if (customer == null)
                return false;

            if (customer.EmailVerified)
                return true; // Already verified

            if (customer.EmailVerificationTokenExpiry < DateTime.UtcNow)
                throw new InvalidOperationException("Verification token has expired");

            customer.EmailVerified = true;
            customer.EmailVerificationToken = null;
            customer.EmailVerificationTokenExpiry = null;
            customer.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await _notification.NotifyAllAsync();
            return true;
        }
    }
}
