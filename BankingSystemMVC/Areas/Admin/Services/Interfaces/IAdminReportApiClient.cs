using BankingSystemMVC.Areas.Admin.Models;

namespace BankingSystemMVC.Areas.Admin.Services.Interfaces
{
    public interface IAdminReportApiClient
    {
        Task<List<AdminReportViewModel>?> GetAllReportsAsync();
        Task<AdminReportViewModel?> GetReportByIdAsync(int id);
        Task<bool> UpdateReportStatusAsync(int id, int status);
        Task<bool> AddMessageAsync(int reportId, string message);
        Task<List<AdminReportMessageViewModel>> GetMessagesAsync(int reportId);
    }
}
