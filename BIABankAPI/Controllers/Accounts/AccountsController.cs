using BankingSystemAPI.Infrastructure.Persistence;
using BankingSystemAPI.Configuration;
using BankingSystemAPI.Domain.Entities.Accounts;
using BankingSystemAPI.Application.Dtos.Accounts;
using BankingSystemAPI.Domain.Entities.Users.Admin;
using BankingSystemAPI.Application.Services.Implementations.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace BankingSystemAPI.Controllers.Accounts
{
    [ApiController]
    [Route("api/accounts")]
    [Authorize]
    public class AccountsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly AuditService _auditService;

        public AccountsController(AppDbContext context, AuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        // GET: api/accounts
        [HttpGet]
        public async Task<IActionResult> GetMyAccounts()
        {
            var customerId = User.GetRequiredUserId();

            var accounts = await _context.Accounts
                .Where(a => a.CustomerId == customerId)
                .Select(a => new AccountSummaryDto
                {
                    Id = a.Id,
                    AccountNumber = a.AccountNumber,
                    AccountName = a.AccountName,
                    AccountType = a.AccountType.ToString(),
                    Balance = a.Balance,
                    Status = a.Status.ToString()
                })
                .ToListAsync();

            return Ok(accounts);
        }

        // POST: api/accounts
        [HttpPost]
        public async Task<IActionResult> CreateAccount(CreateAccountRequestDto dto)
        {
            var customerId = User.GetRequiredUserId();

            // Check if customer has verified KYC
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
                return NotFound("Customer not found");

            if (!customer.IsKYCVerified)
            {
                return BadRequest(new 
                { 
                    error = "KYC verification required",
                    message = "You must complete KYC verification before creating an account. Please upload your identity documents."
                });
            }

            var account = new Account
            {
                CustomerId = customerId,
                AccountName = dto.AccountName,
                AccountType = dto.AccountType,
                AccountNumber = await GenerateUniqueAccountNumberAsync(),
                Status = AccountStatus.Active,
                BalanceInCents = 0
            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            await _auditService.LogAsync(
                AuditAction.AccountCreated,
                "Account",
                account.Id,
                customerId,
                $"Account created with number {account.AccountNumber}"
            );

            return Ok(new { account.Id });
        }

        // GET: api/accounts/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAccountDetail(
            int id,
            [FromQuery] string? transactionType = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] string? reference = null)
        {
            var customerId = User.GetRequiredUserId();

            var account = await _context.Accounts
                .Where(a => a.Id == id && a.CustomerId == customerId)
                .Select(a => new AccountDetailDto
                {
                    Id = a.Id,
                    AccountNumber = a.AccountNumber,
                    AccountName = a.AccountName,
                    AccountType = a.AccountType.ToString(),
                    Balance = a.Balance,
                    Status = a.Status.ToString()
                })
                .FirstOrDefaultAsync();

            if (account == null)
                return NotFound();

            var query = _context.LedgerEntries
                .Where(l => l.AccountId == account.Id);

            // Apply filters
            if (!string.IsNullOrEmpty(transactionType))
            {
                query = query.Where(l => l.Transaction!.Type.ToString() == transactionType);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(l => l.CreatedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                var endDate = toDate.Value.Date.AddDays(1);
                query = query.Where(l => l.CreatedAt < endDate);
            }

            if (!string.IsNullOrEmpty(reference))
            {
                query = query.Where(l => l.Transaction!.TransactionReference.Contains(reference));
            }

            account.RecentTransactions = await query
                .OrderByDescending(l => l.CreatedAt)
                .Take(50)
                .Select(l => new AccountTransactionDto
                {
                    Reference = l.Transaction!.TransactionReference,
                    Date = l.CreatedAt,
                    Type = l.Transaction!.Type.ToString(),
                    Description = l.Description,
                    Amount = l.EntryType == EntryType.Debit
                        ? -l.Amount
                        : l.Amount,
                    PostBalance = l.PostTransactionBalance,
                    Status = l.Transaction!.Status.ToString()
                })
                .ToListAsync();

            return Ok(account);
        }


        // Helpers
        private async Task<string> GenerateUniqueAccountNumberAsync()
        {
            string accountNumber;

            do
            {
                var bytes = RandomNumberGenerator.GetBytes(8);
                var number = Math.Abs(BitConverter.ToInt64(bytes, 0));
                accountNumber = ((number % 90000000000L) + 10000000000L).ToString();
            }
            while (await _context.Accounts.AnyAsync(a => a.AccountNumber == accountNumber));

            return accountNumber;
        }
    }
}