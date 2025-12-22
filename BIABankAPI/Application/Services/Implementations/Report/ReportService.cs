using BankingSystemAPI.Application.Services.Interfaces.Report;
using BankingSystemAPI.Domain.Entities.Reports;
using BankingSystemAPI.Infrastructure.Persistence;
using BankingSystemAPI.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace BankingSystemAPI.Application.Services.Implementations.Report
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext _context;
        private readonly NotificationService _notification;

        public ReportService(AppDbContext context, NotificationService notification)
        {
            _context = context;
            _notification = notification;
        }

        public async Task<Domain.Entities.Reports.Report> CreateReportAsync(int customerId, string name, string title, string description)
        {
            var hasActiveReport = await _context.Reports
                .AnyAsync(r => r.CustomerId == customerId && 
                              r.Status != ReportStatus.Resolved && 
                              r.Status != ReportStatus.Closed);

            if (hasActiveReport)
            {
                throw new InvalidOperationException("You already have an active report. Please wait until it is resolved or closed before creating a new one.");
            }

            var report = new Domain.Entities.Reports.Report
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
            await _notification.NotifyAllAsync();

            return report;
        }

        public async Task<List<Domain.Entities.Reports.Report>> GetCustomerReportsAsync(int customerId)
        {
            return await _context.Reports
                .Where(r => r.CustomerId == customerId)
                .Include(r => r.Customer)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Domain.Entities.Reports.Report>> GetAllReportsAsync()
        {
            return await _context.Reports
                .Include(r => r.Customer)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<Domain.Entities.Reports.Report?> GetReportByIdAsync(int reportId)
        {
            return await _context.Reports
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(r => r.Id == reportId);
        }

        public async Task<Domain.Entities.Reports.Report?> UpdateReportStatusAsync(int reportId, ReportStatus status)
        {
            var report = await _context.Reports.FindAsync(reportId);
            if (report == null)
                return null;

            report.Status = status;
            report.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await _notification.NotifyAllAsync();
            return report;
        }

        public async Task<ReportMessage> AddMessageAsync(int reportId, string message, MessageSenderType senderType, int? customerId, int? adminUserId)
        {
            var reportMessage = new ReportMessage
            {
                ReportId = reportId,
                Message = message,
                SenderType = senderType,
                CustomerId = customerId,
                AdminUserId = adminUserId,
                CreatedAt = DateTime.UtcNow
            };

            _context.ReportMessages.Add(reportMessage);
            await _context.SaveChangesAsync();
            await _notification.NotifyAllAsync();

            return reportMessage;
        }

        public async Task<List<ReportMessage>> GetReportMessagesAsync(int reportId)
        {
            return await _context.ReportMessages
                .Where(rm => rm.ReportId == reportId)
                .Include(rm => rm.Customer)
                .Include(rm => rm.AdminUser)
                .OrderBy(rm => rm.CreatedAt)
                .ToListAsync();
        }
    }
}
