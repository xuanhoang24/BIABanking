using BankingSystemAPI.DataLayer;
using BankingSystemAPI.Models.Reports;
using BankingSystemAPI.Services.Report.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankingSystemAPI.Services.Report.Implements
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext _context;

        public ReportService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Models.Reports.Report> CreateReportAsync(int customerId, string name, string title, string description)
        {
            var report = new Models.Reports.Report
            {
                CustomerId = customerId,
                Name = name,
                Title = title,
                Description = description,
                Status = ReportStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Reports.Add(report);
            await _context.SaveChangesAsync();

            return report;
        }

        public async Task<List<Models.Reports.Report>> GetCustomerReportsAsync(int customerId)
        {
            return await _context.Reports
                .Where(r => r.CustomerId == customerId)
                .Include(r => r.Customer)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Models.Reports.Report>> GetAllReportsAsync()
        {
            return await _context.Reports
                .Include(r => r.Customer)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<Models.Reports.Report?> GetReportByIdAsync(int reportId)
        {
            return await _context.Reports
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(r => r.Id == reportId);
        }

        public async Task<Models.Reports.Report?> UpdateReportStatusAsync(int reportId, ReportStatus status)
        {
            var report = await _context.Reports.FindAsync(reportId);
            if (report == null)
                return null;

            report.Status = status;
            report.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return report;
        }
    }
}
