using BankingSystemAPI.Application.Services.Implementations.Admin;
using BankingSystemAPI.Application.Services.Interfaces.Customer;
using BankingSystemAPI.Domain.Entities.Accounts;
using BankingSystemAPI.Domain.Entities.Users.Admin;
using BankingSystemAPI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace BankingSystemAPI.Application.Services.Implementations.Customer
{
    public class TransactionService : ITransactionService
    {
        private readonly AppDbContext _context;
        private readonly AuditService _audit;

        public TransactionService(AppDbContext context, AuditService audit)
        {
            _context = context;
            _audit = audit;
        }

        // DEPOSIT
        public async Task<string> DepositAsync(int customerId, int accountId, decimal amount, string? description)
        {
            var amountInCents = ValidateAmount(amount);

            using var tx = await _context.Database.BeginTransactionAsync();

            var account = await GetOwnedAccount(accountId, customerId);

            account.BalanceInCents += amountInCents;

            var transaction = CreateTransaction(
                TransactionType.Deposit,
                amountInCents,
                toAccount: account,
                description: description
            );

            await SaveTransactionAsync(transaction);

            await AddLedgerEntry(
                transaction.Id,
                account,
                EntryType.Credit,
                amountInCents,
                "Deposit"
            );

            await LogAsync($"Deposit of ${amount:F2} to account {account.AccountNumber} (Ref: {transaction.TransactionReference})",
                transaction.Id,
                customerId);

            await tx.CommitAsync();

            return transaction.TransactionReference;
        }

        // WITHDRAW
        public async Task<string> WithdrawAsync(int customerId, int accountId, decimal amount, string? description)
        {
            var amountInCents = ValidateAmount(amount);

            using var tx = await _context.Database.BeginTransactionAsync();

            var account = await GetOwnedAccount(accountId, customerId);

            EnsureSufficientBalance(account, amountInCents);

            account.BalanceInCents -= amountInCents;

            var transaction = CreateTransaction(
                TransactionType.Withdrawal,
                amountInCents,
                fromAccount: account,
                description: description
            );

            await SaveTransactionAsync(transaction);

            await AddLedgerEntry(
                transaction.Id,
                account,
                EntryType.Debit,
                amountInCents,
                "Withdrawal"
            );

            await LogAsync(
                $"Withdrawal of ${amount:F2} from account {account.AccountNumber} (Ref: {transaction.TransactionReference})",
                transaction.Id,
                customerId);

            await tx.CommitAsync();

            return transaction.TransactionReference;
        }

        // TRANSFER
        public async Task<string> TransferAsync(int customerId, int fromAccountId, string toAccountNumber, decimal amount, string? description)
        {
            var amountInCents = ValidateAmount(amount);

            using var tx = await _context.Database.BeginTransactionAsync();

            var fromAccount = await _context.Accounts
                .FirstOrDefaultAsync(a =>
                    a.Id == fromAccountId &&
                    a.CustomerId == customerId)
                ?? throw new UnauthorizedAccessException("Invalid source account");

            var toAccount = await _context.Accounts
                .FirstOrDefaultAsync(a => a.AccountNumber == toAccountNumber)
                ?? throw new InvalidOperationException("Destination account not found");

            if (fromAccount.Id == toAccount.Id)
                throw new InvalidOperationException("Cannot transfer to the same account");

            EnsureSufficientBalance(fromAccount, amountInCents);

            fromAccount.BalanceInCents -= amountInCents;
            toAccount.BalanceInCents += amountInCents;

            var transaction = new Transaction
            {
                TransactionReference = GenerateReference(GetPrefix(TransactionType.Transfer)),
                Type = TransactionType.Transfer,
                Status = TransactionStatus.Completed,
                AmountInCents = amountInCents,
                Description = description ?? "Transfer",
                FromCustomerId = fromAccount.CustomerId,
                ToCustomerId = toAccount.CustomerId,
                FromAccountId = fromAccount.Id,
                ToAccountId = toAccount.Id,
                ProcessedAt = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            _context.LedgerEntries.AddRange(
                new LedgerEntry
                {
                    TransactionId = transaction.Id,
                    AccountId = fromAccount.Id,
                    EntryType = EntryType.Debit,
                    AmountInCents = amountInCents,
                    PostTransactionBalanceInCents = fromAccount.BalanceInCents,
                    Description = "Transfer debit"
                },
                new LedgerEntry
                {
                    TransactionId = transaction.Id,
                    AccountId = toAccount.Id,
                    EntryType = EntryType.Credit,
                    AmountInCents = amountInCents,
                    PostTransactionBalanceInCents = toAccount.BalanceInCents,
                    Description = "Transfer credit"
                }
            );

            await _context.SaveChangesAsync();

            await _audit.LogAsync(
                AuditAction.TransactionCompleted,
                "Transaction",
                transaction.Id,
                customerId,
                $"Transfer of ${amount:F2} from account {fromAccount.AccountNumber} to {toAccount.AccountNumber} (Ref: {transaction.TransactionReference})"
            );

            await tx.CommitAsync();

            return transaction.TransactionReference;
        }


        // PAYMENT
        public async Task<string> PaymentAsync(int customerId, int accountId, decimal amount, string merchant)
        {
            var amountInCents = ValidateAmount(amount);

            using var tx = await _context.Database.BeginTransactionAsync();

            var account = await GetOwnedAccount(accountId, customerId);

            EnsureSufficientBalance(account, amountInCents);

            account.BalanceInCents -= amountInCents;

            var transaction = CreateTransaction(
                TransactionType.Fee,
                amountInCents,
                fromAccount: account,
                metadata: $"Merchant: {merchant}"
            );

            await SaveTransactionAsync(transaction);

            await AddLedgerEntry(
                transaction.Id,
                account,
                EntryType.Debit,
                amountInCents,
                $"Payment to {merchant}"
            );

            await LogAsync(
                $"Payment of ${amount:F2} to {merchant} from account {account.AccountNumber} (Ref: {transaction.TransactionReference})",
                transaction.Id,
                customerId
            );

            await tx.CommitAsync();

            return transaction.TransactionReference;
        }

        // HELPERS
        private async Task<Account> GetOwnedAccount(int accountId, int customerId)
        {
            return await _context.Accounts
                .FirstOrDefaultAsync(a => a.Id == accountId && a.CustomerId == customerId)
                ?? throw new UnauthorizedAccessException("Account not owned by customer");
        }

        private static long ValidateAmount(decimal amount)
        {
            if (amount <= 0)
                throw new InvalidOperationException("Invalid amount");

            return (long)(amount * 100);
        }

        private static void EnsureSufficientBalance(Account account, long amountInCents)
        {
            if (account.BalanceInCents < amountInCents)
                throw new InvalidOperationException("Insufficient balance");
        }

        private async Task SaveTransactionAsync(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
        }

        private async Task AddLedgerEntry(int transactionId, Account account, EntryType type, long amountInCents, string description)
        {
            _context.LedgerEntries.Add(new LedgerEntry
            {
                TransactionId = transactionId,
                AccountId = account.Id,
                EntryType = type,
                AmountInCents = amountInCents,
                PostTransactionBalanceInCents = account.BalanceInCents,
                Description = description
            });

            await _context.SaveChangesAsync();
        }

        private async Task LogAsync(string message, int transactionId, int customerId)
        {
            await _audit.LogAsync(
                AuditAction.TransactionCompleted,
                "Transaction",
                transactionId,
                customerId,
                message
            );
        }

        private static Transaction CreateTransaction(
            TransactionType type,
            long amountInCents,
            Account? fromAccount = null,
            Account? toAccount = null,
            string? description = null,
            string? metadata = null)
        {
            return new Transaction
            {
                TransactionReference = GenerateReference(GetPrefix(type)),
                Type = type,
                Status = TransactionStatus.Completed,
                AmountInCents = amountInCents,
                Description = description ?? type.ToString(),
                FromCustomerId = fromAccount?.CustomerId,
                ToCustomerId = toAccount?.CustomerId,
                FromAccountId = fromAccount?.Id,
                ToAccountId = toAccount?.Id,
                Metadata = metadata,
                ProcessedAt = DateTime.UtcNow
            };
        }

        private static string GenerateReference(string prefix = "TX")
        {
            var datePart = DateTime.UtcNow.ToString("yyyyMMdd");

            var randomBytes = RandomNumberGenerator.GetBytes(4); // 8 hex chars
            var hexPart = Convert.ToHexString(randomBytes);

            return $"{prefix}-{datePart}-{hexPart}";
        }

        private static string GetPrefix(TransactionType type)
        {
            return type switch
            {
                TransactionType.Deposit => "DEP",
                TransactionType.Withdrawal => "WIT",
                TransactionType.Transfer => "TXN",
                TransactionType.Fee => "FEE",
                TransactionType.Interest => "INT",
                TransactionType.Refund => "REF",
                _ => "TXN"
            };
        }
    }
}
