using BankingSystemMVC.Areas.Admin.Models.ViewModels.Dashboard;

namespace BankingSystemMVC.Areas.Admin.Services.Interfaces.Dashboard
{
    public interface IAdminDashboardApiClient
    {
        Task<DashboardStatsViewModel?> GetDashboardStatsAsync();
    }
}