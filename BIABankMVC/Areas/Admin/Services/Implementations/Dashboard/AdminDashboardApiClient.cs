using BankingSystemMVC.Areas.Admin.Models.ViewModels.Dashboard;
using BankingSystemMVC.Areas.Admin.Services.Interfaces.Dashboard;
using BankingSystemMVC.Infrastructure.Json;

namespace BankingSystemMVC.Areas.Admin.Services.Implementations.Dashboard
{
    public class AdminDashboardApiClient : IAdminDashboardApiClient
    {
        private readonly HttpClient _httpClient;

        public AdminDashboardApiClient(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("AdminApi");
        }

        public async Task<DashboardStatsViewModel?> GetDashboardStatsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/admin/customers/stats");
                
                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                return JsonHelper.Deserialize<DashboardStatsViewModel>(json);
            }
            catch
            {
                return null;
            }
        }
    }
}