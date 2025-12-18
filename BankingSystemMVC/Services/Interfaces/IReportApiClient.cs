using BankingSystemMVC.Models.Reports;

namespace BankingSystemMVC.Services.Interfaces
{
    public interface IReportApiClient
    {
        Task<(bool Success, string? Error)> CreateReportAsync(CreateReportViewModel model);
        Task<List<ReportViewModel>> GetMyReportsAsync();
        Task<ReportViewModel?> GetReportByIdAsync(int id);
        Task<(bool Success, string? Error)> AddMessageAsync(int reportId, string message);
        Task<List<ReportMessageViewModel>> GetMessagesAsync(int reportId);
    }
}
