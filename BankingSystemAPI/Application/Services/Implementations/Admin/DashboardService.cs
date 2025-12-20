using BankingSystemAPI.Application.Dtos.Admin;
using BankingSystemAPI.Application.Services.Interfaces.Admin;
using BankingSystemAPI.Domain.Entities.Accounts;
using BankingSystemAPI.Domain.Entities.Users.Customers;
using BankingSystemAPI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankingSystemAPI.Application.Services.Implementations.Admin
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _context;

        public DashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            var totalCustomers = await _context.Customers.CountAsync();

            var activeAccounts = await _context.Accounts
                .Where(a => a.Status == AccountStatus.Active)
                .CountAsync();

            var pendingKYC = await _context.KYCDocuments
                .Where(k => k.Status == KYCStatus.Pending || k.Status == KYCStatus.UnderReview)
                .CountAsync();

            // Calculate Net Flow Today = Deposits - Withdrawals - External Transfers - Payments
            // External transfers are transfers where FromCustomerId != ToCustomerId
            // Convert local "today" to UTC range for querying
            var todayLocalStart = DateTime.Today; // Start of today in local time
            var todayLocalEnd = todayLocalStart.AddDays(1); // Start of tomorrow in local time
            var todayUtcStart = todayLocalStart.ToUniversalTime();
            var todayUtcEnd = todayLocalEnd.ToUniversalTime();

            var todayTransactions = await _context.Transactions
                .Where(t => t.CreatedAt >= todayUtcStart && t.CreatedAt < todayUtcEnd && t.Status == TransactionStatus.Completed)
                .ToListAsync();

            var totalDeposits = todayTransactions
                .Where(t => t.Type == TransactionType.Deposit)
                .Sum(t => (decimal)t.AmountInCents / 100);

            var totalWithdrawals = todayTransactions
                .Where(t => t.Type == TransactionType.Withdrawal)
                .Sum(t => (decimal)t.AmountInCents / 100);

            // Only count external transfers (to different customers)
            var totalExternalTransfers = todayTransactions
                .Where(t => t.Type == TransactionType.Transfer && t.FromCustomerId != t.ToCustomerId)
                .Sum(t => (decimal)t.AmountInCents / 100);

            var totalPayments = todayTransactions
                .Where(t => t.Type == TransactionType.Fee)
                .Sum(t => (decimal)t.AmountInCents / 100);

            var netFlowToday = totalDeposits - totalWithdrawals - totalExternalTransfers - totalPayments;

            return new DashboardStatsDto
            {
                TotalCustomers = totalCustomers,
                ActiveAccounts = activeAccounts,
                PendingKYC = pendingKYC,
                NetFlowToday = netFlowToday
            };
        }
    }
}