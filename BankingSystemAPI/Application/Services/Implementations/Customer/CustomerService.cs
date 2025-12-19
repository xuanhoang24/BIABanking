using BankingSystemAPI.Application.Dtos.Customers;
using BankingSystemAPI.Application.Services.Implementations.Admin;
using BankingSystemAPI.Application.Services.Interfaces.Customer;
using BankingSystemAPI.Infrastructure.Persistence;
using BankingSystemAPI.Infrastructure.Security.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankingSystemAPI.Application.Services.Implementations.Customer
{
    public class CustomerService : ICustomerService
    {
        private readonly AppDbContext _context;
        private readonly AuditService _auditService;
        private readonly IPasswordHasher _passwordHasher;

        public CustomerService( AppDbContext context, AuditService auditService, IPasswordHasher passwordHasher)
        {
            _context = context;
            _auditService = auditService;
            _passwordHasher = passwordHasher;
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

            var customer = new Domain.Entities.Users.Customers.Customer
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

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return customer;
        }

        public async Task<Domain.Entities.Users.Customers.Customer?> AuthenticateCustomerAsync(string email, string password)
        {
            email = email.ToLowerInvariant();

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == email);
            if (customer == null)
                return null;

            if (!_passwordHasher.Verify(password, customer.PasswordHash, customer.PasswordSalt))
                return null;

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
            customer.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
