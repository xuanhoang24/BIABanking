using BankingSystemMVC.Areas.Admin.Models;
using BankingSystemMVC.Areas.Admin.Services.Interfaces;
using System.Text;
using System.Text.Json;

namespace BankingSystemMVC.Areas.Admin.Services.Implements
{
    public class AdminReportApiClient : IAdminReportApiClient
    {
        private readonly HttpClient _client;

        public AdminReportApiClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<List<AdminReportViewModel>?> GetAllReportsAsync()
        {
            var response = await _client.GetAsync("api/admin/reports");

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<AdminReportViewModel>>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
        }

        public async Task<AdminReportViewModel?> GetReportByIdAsync(int id)
        {
            var response = await _client.GetAsync($"api/admin/reports/{id}");

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<AdminReportViewModel>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
        }

        public async Task<bool> UpdateReportStatusAsync(int id, int status)
        {
            var payload = new { Status = status };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PatchAsync($"api/admin/reports/{id}/status", content);

            return response.IsSuccessStatusCode;
        }
    }
}
