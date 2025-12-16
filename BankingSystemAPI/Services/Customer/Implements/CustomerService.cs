using BankingSystemAPI.DataLayer;
using BankingSystemAPI.Models.DTOs.Customer;
using BankingSystemAPI.Models.Users.Admin;
using BankingSystemAPI.Security.Interfaces;
using BankingSystemAPI.Services.Admin.Implements;
using BankingSystemAPI.Services.Customer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankingSystemAPI.Services.Customer.Implements
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

        public async Task<BankingSystemAPI.Models.Users.Customers.Customer> RegisterCustomerAsync(
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

            var customer = new BankingSystemAPI.Models.Users.Customers.Customer
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

            await _auditService.LogAsync(
                AuditAction.CustomerRegistration,
                "Customer",
                customer.Id,
                customer.Id,
                $"Customer registered: {email}"
            );

            await transaction.CommitAsync();
            return customer;
        }

        public async Task<Models.Users.Customers.Customer?> AuthenticateCustomerAsync(string email, string password)
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
    }
}
