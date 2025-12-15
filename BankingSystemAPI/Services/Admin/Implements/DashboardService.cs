using BankingSystemAPI.DataLayer;
using BankingSystemAPI.Models;
using BankingSystemAPI.Models.DTOs.Admin;
using BankingSystemAPI.Services.Admin.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankingSystemAPI.Services.Admin.Implements
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
            
            var pendingKYC = await _context.Customers
                .Where(u => !u.IsKYCVerified)
                .CountAsync();
            
            var todaysTransactions = await _context.Transactions
                .Where(t => t.CreatedAt.Date == DateTime.Today)
                .CountAsync();

            return new DashboardStatsDto
            {
                TotalCustomers = totalCustomers,
                ActiveAccounts = activeAccounts,
                PendingKYC = pendingKYC,
                TodaysTransactions = todaysTransactions
            };
        }
    }
}