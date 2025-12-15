using BankingSystemMVC.Areas.Admin.Models;
using BankingSystemMVC.Areas.Admin.Services.Interfaces;
using System.Text.Json;

namespace BankingSystemMVC.Areas.Admin.Services.Implements
{
    public class AdminDashboardApiClient : IAdminDashboardApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public AdminDashboardApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<DashboardStatsViewModel?> GetDashboardStatsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/admin/customers/stats");
                
                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                var stats = JsonSerializer.Deserialize<DashboardStatsViewModel>(json, _jsonOptions);
                
                return stats;
            }
            catch
            {
                return null;
            }
        }
    }
}