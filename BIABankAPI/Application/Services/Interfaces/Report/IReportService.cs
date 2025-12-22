using BankingSystemAPI.Domain.Entities.Reports;

namespace BankingSystemAPI.Application.Services.Interfaces.Report
{
    public interface IReportService
    {
        Task<Domain.Entities.Reports.Report> CreateReportAsync(int customerId, string name, string title, string description);
        Task<List<Domain.Entities.Reports.Report>> GetCustomerReportsAsync(int customerId);
        Task<List<Domain.Entities.Reports.Report>> GetAllReportsAsync();
        Task<Domain.Entities.Reports.Report?> GetReportByIdAsync(int reportId);
        Task<Domain.Entities.Reports.Report?> UpdateReportStatusAsync(int reportId, ReportStatus status);
        Task<ReportMessage> AddMessageAsync(int reportId, string message, MessageSenderType senderType, int? customerId, int? adminUserId);
        Task<List<ReportMessage>> GetReportMessagesAsync(int reportId);
    }
}
