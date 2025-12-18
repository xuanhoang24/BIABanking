using BankingSystemAPI.Models.Reports;

namespace BankingSystemAPI.Services.Report.Interfaces
{
    public interface IReportService
    {
        Task<Models.Reports.Report> CreateReportAsync(int customerId, string name, string title, string description);
        Task<List<Models.Reports.Report>> GetCustomerReportsAsync(int customerId);
        Task<List<Models.Reports.Report>> GetAllReportsAsync();
        Task<Models.Reports.Report?> GetReportByIdAsync(int reportId);
        Task<Models.Reports.Report?> UpdateReportStatusAsync(int reportId, ReportStatus status);
        Task<ReportMessage> AddMessageAsync(int reportId, string message, MessageSenderType senderType, int? customerId, int? adminUserId);
        Task<List<ReportMessage>> GetReportMessagesAsync(int reportId);
    }
}
