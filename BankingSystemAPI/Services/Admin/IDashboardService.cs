using BankingSystemAPI.Models.DTOs.Admin;

namespace BankingSystemAPI.Services.Admin
{
    public interface IDashboardService
    {
        Task<DashboardStatsDto> GetDashboardStatsAsync();
    }
}