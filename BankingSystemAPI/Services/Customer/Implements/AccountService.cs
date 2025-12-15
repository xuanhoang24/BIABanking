using BankingSystemAPI.DataLayer;
using BankingSystemAPI.Models;
using BankingSystemAPI.Models.Users.Admin;
using BankingSystemAPI.Services.Admin.Implements;
using BankingSystemAPI.Services.Customer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankingSystemAPI.Services.Customer.Implements
{
    public class AccountService : IAccountService
    {
        private readonly AppDbContext _context;
        private readonly AuditService _auditService;

        public AccountService(AppDbContext context, AuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        public async Task<Account?> DepositAsync(int accountId, int customerId, long amountInCents, string? description)
        {
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a =>
                    a.Id == accountId &&
                    a.CustomerId == customerId);

            if (account == null)
                return null;

            if (account.Status != AccountStatus.Active)
                throw new InvalidOperationException("Account is not active");

            using var transaction = await _context.Database.BeginTransactionAsync();

            var bankingTransaction = new Transaction
            {
                TransactionReference = Guid.NewGuid().ToString("N"),
                Type = TransactionType.Deposit,
                Status = TransactionStatus.Completed,
                AmountInCents = amountInCents,
                Description = description ?? "Deposit",
                ToCustomerId = customerId,
                ToAccountId = account.Id,
                CreatedAt = DateTime.UtcNow,
                ProcessedAt = DateTime.UtcNow
            };

            _context.Transactions.Add(bankingTransaction);
            await _context.SaveChangesAsync();

            account.BalanceInCents += amountInCents;
            account.UpdatedAt = DateTime.UtcNow;

            var ledger = new LedgerEntry
            {
                TransactionId = bankingTransaction.Id,
                AccountId = account.Id,
                EntryType = EntryType.Credit,
                AmountInCents = amountInCents,
                Description = "Deposit",
                PostTransactionBalanceInCents = account.BalanceInCents
            };

            _context.LedgerEntries.Add(ledger);
            await _context.SaveChangesAsync();

            await _auditService.LogAsync(
                AuditAction.TransactionCompleted,
                "Account",
                account.Id,
                customerId,
                $"Deposit {amountInCents / 100.0m} to account {account.AccountNumber}"
            );

            await transaction.CommitAsync();

            return account;
        }
    }
}
