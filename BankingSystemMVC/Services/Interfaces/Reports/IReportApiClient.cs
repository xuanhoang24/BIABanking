using BankingSystemMVC.Models.ViewModels.Reports;

namespace BankingSystemMVC.Services.Interfaces.Reports
{
    public interface IReportApiClient
    {
        Task<(bool Success, string? Error)> CreateReportAsync(CreateReportViewModel model);
        Task<List<ReportViewModel>> GetMyReportsAsync();
        Task<ReportViewModel?> GetReportByIdAsync(int id);
        Task<(bool Success, string? Error)> AddMessageAsync(int reportId, string message);
        Task<List<ReportMessageViewModel>> GetMessagesAsync(int reportId);
        Task<(bool Success, string? Error)> CloseReportAsync(int reportId);
    }
}
