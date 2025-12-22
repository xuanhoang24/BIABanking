using BankingSystemAPI.Application.Dtos.Admin;

namespace BankingSystemAPI.Application.Services.Interfaces.Admin
{
    public interface IDashboardService
    {
        Task<DashboardStatsDto> GetDashboardStatsAsync();
    }
}