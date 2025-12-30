using BankingSystemAPI.Application.Dtos.Accounts;
using BankingSystemAPI.Application.Dtos.Admin;
using BankingSystemAPI.Application.Services.Interfaces.Admin;
using BankingSystemAPI.Domain.Entities.Users.Admin;
using BankingSystemAPI.Domain.Entities.Users.Customers;
using BankingSystemAPI.Infrastructure.Persistence;
using BankingSystemAPI.Infrastructure.Security.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankingSystemAPI.Application.Services.Implementations.Admin
{
    public class CustomerAdminService : ICustomerAdminService
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly AuditService _auditService;

        public CustomerAdminService(AppDbContext context, IPasswordHasher passwordHasher, AuditService auditService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _auditService = auditService;
        }

        public async Task<List<CustomerListDto>> GetAllCustomersAsync()
        {
            var customers = await _context.Customers
                .Include(c => c.Accounts)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CustomerListDto
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Email = c.Email,
                    PhoneNumber = c.PhoneNumber,
                    IsKYCVerified = c.IsKYCVerified,
                    Status = c.Status.ToString(),
                    AccountCount = c.Accounts.Count,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

            return customers;
        }

        public async Task<CustomerDetailDto?> GetCustomerByIdAsync(int customerId)
        {
            var customer = await _context.Customers
                .Include(c => c.Accounts)
                .Include(c => c.SentTransactions)
                    .ThenInclude(t => t.ToAccount)
                .Include(c => c.ReceivedTransactions)
                    .ThenInclude(t => t.FromAccount)
                .FirstOrDefaultAsync(c => c.Id == customerId);

            if (customer == null)
                return null;

            var accounts = customer.Accounts.Select(a => new AccountSummaryDto
            {
                Id = a.Id,
                AccountNumber = a.AccountNumber,
                AccountType = a.AccountType.ToString(),
                Status = a.Status.ToString(),
                Balance = a.Balance,
                AccountName = a.AccountName,
                CreatedAt = a.CreatedAt
            }).ToList();

            var transactions = customer.SentTransactions
                .Concat(customer.ReceivedTransactions)
                .OrderByDescending(t => t.CreatedAt)
                .Take(10)
                .Select(t => new TransactionSummaryDto
                {
                    Id = t.Id,
                    TransactionReference = t.TransactionReference,
                    Type = t.Type.ToString(),
                    Status = t.Status.ToString(),
                    Amount = t.Amount,
                    Description = t.Description,
                    FromAccount = t.FromAccount?.AccountNumber,
                    ToAccount = t.ToAccount?.AccountNumber,
                    CreatedAt = t.CreatedAt
                })
                .ToList();

            return new CustomerDetailDto
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                DateOfBirth = customer.DateOfBirth,
                Address = customer.Address,
                IsKYCVerified = customer.IsKYCVerified,
                Status = customer.Status.ToString(),
                FailedLoginAttempts = customer.FailedLoginAttempts,
                LockedUntil = customer.LockedUntil,
                CreatedAt = customer.CreatedAt,
                UpdatedAt = customer.UpdatedAt,
                Accounts = accounts,
                RecentTransactions = transactions
            };
        }

        public async Task<List<AccountSummaryDto>> GetCustomerAccountsAsync(int customerId)
        {
            var accounts = await _context.Accounts
                .Where(a => a.CustomerId == customerId)
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => new AccountSummaryDto
                {
                    Id = a.Id,
                    AccountNumber = a.AccountNumber,
                    AccountType = a.AccountType.ToString(),
                    Status = a.Status.ToString(),
                    Balance = a.Balance,
                    AccountName = a.AccountName,
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync();

            return accounts;
        }

        public async Task<List<TransactionSummaryDto>> GetCustomerTransactionsAsync(int customerId, int limit = 50)
        {
            var transactions = await _context.Transactions
                .Where(t => t.FromCustomerId == customerId || t.ToCustomerId == customerId)
                .Include(t => t.FromAccount)
                .Include(t => t.ToAccount)
                .OrderByDescending(t => t.CreatedAt)
                .Take(limit)
                .Select(t => new TransactionSummaryDto
                {
                    Id = t.Id,
                    TransactionReference = t.TransactionReference,
                    Type = t.Type.ToString(),
                    Status = t.Status.ToString(),
                    Amount = t.Amount,
                    Description = t.Description,
                    FromAccount = t.FromAccount != null ? t.FromAccount.AccountNumber : null,
                    ToAccount = t.ToAccount != null ? t.ToAccount.AccountNumber : null,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();

            return transactions;
        }

        public async Task<bool> UpdateCustomerStatusAsync(int customerId, string status)
        {
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
                return false;

            if (Enum.TryParse<CustomerStatus>(status, out var customerStatus))
            {
                customer.Status = customerStatus;
                customer.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> ResetCustomerPasswordAsync(int customerId)
        {
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
                return false;

            // Generate password: firstname + MMddyy
            var newPassword = $"{customer.FirstName.ToLower()}{customer.DateOfBirth:MMddyy}";

            _passwordHasher.CreateHash(newPassword, out var hash, out var salt);

            customer.PasswordHash = hash;
            customer.PasswordSalt = salt;
            customer.RequiresPasswordChange = true;
            customer.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCustomerAsync(int customerId)
        {
            var customer = await _context.Customers
                .Include(c => c.Accounts)
                .Include(c => c.SentTransactions)
                .Include(c => c.ReceivedTransactions)
                .FirstOrDefaultAsync(c => c.Id == customerId);

            if (customer == null)
                return false;

            var customerName = $"{customer.FirstName} {customer.LastName}";
            var customerEmail = customer.Email;
            var accountCount = customer.Accounts.Count;
            var transactionCount = customer.SentTransactions.Count + customer.ReceivedTransactions.Count;

            // Delete related reports
            var reports = await _context.Reports.Where(r => r.CustomerId == customerId).ToListAsync();
            _context.Reports.RemoveRange(reports);

            // Delete related entities
            _context.Transactions.RemoveRange(customer.SentTransactions);
            _context.Transactions.RemoveRange(customer.ReceivedTransactions);
            _context.Accounts.RemoveRange(customer.Accounts);
            _context.Customers.Remove(customer);

            await _context.SaveChangesAsync();

            // Log audit action
            await _auditService.LogAsync(
                AuditAction.CustomerDeleted,
                "Customer",
                customerId,
                null, // No admin user ID available in service layer
                $"Deleted customer {customerName} ({customerEmail}) with {accountCount} accounts and {transactionCount} transactions"
            );

            return true;
        }

        public async Task<List<AccountListDto>> GetAllAccountsAsync()
        {
            var accounts = await _context.Accounts
                .Include(a => a.Customer)
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => new AccountListDto
                {
                    Id = a.Id,
                    AccountNumber = a.AccountNumber,
                    AccountName = a.AccountName,
                    AccountType = a.AccountType.ToString(),
                    Status = a.Status.ToString(),
                    Balance = a.Balance,
                    CustomerId = a.CustomerId,
                    CustomerName = a.Customer != null ? $"{a.Customer.FirstName} {a.Customer.LastName}" : "Unknown",
                    CustomerEmail = a.Customer != null ? a.Customer.Email : "",
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync();

            return accounts;
        }

        public async Task<List<TransactionListDto>> GetAllTransactionsAsync(TransactionFilterDto filter)
        {
            var query = _context.Transactions
                .Include(t => t.FromCustomer)
                .Include(t => t.ToCustomer)
                .Include(t => t.FromAccount)
                .Include(t => t.ToAccount)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(filter.TransactionType))
            {
                query = query.Where(t => t.Type.ToString() == filter.TransactionType);
            }

            if (!string.IsNullOrEmpty(filter.Status))
            {
                query = query.Where(t => t.Status.ToString() == filter.Status);
            }

            if (!string.IsNullOrEmpty(filter.Reference))
            {
                query = query.Where(t => t.TransactionReference.Contains(filter.Reference));
            }

            if (filter.FromDate.HasValue)
            {
                query = query.Where(t => t.CreatedAt >= filter.FromDate.Value);
            }

            if (filter.ToDate.HasValue)
            {
                var toDateEnd = filter.ToDate.Value.AddDays(1);
                query = query.Where(t => t.CreatedAt < toDateEnd);
            }

            var transactions = await query
                .OrderByDescending(t => t.CreatedAt)
                .Take(filter.Limit)
                .Select(t => new TransactionListDto
                {
                    Id = t.Id,
                    TransactionReference = t.TransactionReference,
                    Type = t.Type.ToString(),
                    Status = t.Status.ToString(),
                    Amount = t.Amount,
                    Description = t.Description,
                    FromCustomerId = t.FromCustomerId,
                    FromCustomerName = t.FromCustomer != null ? $"{t.FromCustomer.FirstName} {t.FromCustomer.LastName}" : null,
                    FromAccountNumber = t.FromAccount != null ? t.FromAccount.AccountNumber : null,
                    ToCustomerId = t.ToCustomerId,
                    ToCustomerName = t.ToCustomer != null ? $"{t.ToCustomer.FirstName} {t.ToCustomer.LastName}" : null,
                    ToAccountNumber = t.ToAccount != null ? t.ToAccount.AccountNumber : null,
                    CreatedAt = t.CreatedAt,
                    ProcessedAt = t.ProcessedAt
                })
                .ToListAsync();

            return transactions;
        }
    }
}
