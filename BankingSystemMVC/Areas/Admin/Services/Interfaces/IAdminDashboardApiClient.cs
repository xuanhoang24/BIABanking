using BankingSystemMVC.Areas.Admin.Models;

namespace BankingSystemMVC.Areas.Admin.Services.Interfaces
{
    public interface IAdminDashboardApiClient
    {
        Task<DashboardStatsViewModel?> GetDashboardStatsAsync();
    }
}