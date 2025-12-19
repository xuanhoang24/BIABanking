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

            var todayVolume = await _context.Transactions
                .Where(t => t.CreatedAt.Date == DateTime.Today && t.Status == TransactionStatus.Completed)
                .SumAsync(t => (decimal?)t.AmountInCents / 100) ?? 0;

            return new DashboardStatsDto
            {
                TotalCustomers = totalCustomers,
                ActiveAccounts = activeAccounts,
                PendingKYC = pendingKYC,
                TodayVolume = todayVolume
            };
        }
    }
}