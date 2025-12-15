using BankingSystemAPI.Models.DTOs.Admin;

namespace BankingSystemAPI.Services.Admin.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardStatsDto> GetDashboardStatsAsync();
    }
}