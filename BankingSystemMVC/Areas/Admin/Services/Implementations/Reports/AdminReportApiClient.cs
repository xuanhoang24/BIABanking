using BankingSystemMVC.Areas.Admin.Models.ViewModels.Reports;
using BankingSystemMVC.Areas.Admin.Services.Interfaces.Reports;
using BankingSystemMVC.Infrastructure.Json;
using System.Text;
using System.Text.Json;

namespace BankingSystemMVC.Areas.Admin.Services.Implementations.Reports
{
    public class AdminReportApiClient : IAdminReportApiClient
    {
        private readonly HttpClient _client;

        public AdminReportApiClient(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("AdminApi");
        }

        public async Task<List<AdminReportViewModel>?> GetAllReportsAsync()
        {
            var response = await _client.GetAsync("api/admin/reports");

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();

            return JsonHelper.Deserialize<List<AdminReportViewModel>>(json);
        }

        public async Task<AdminReportViewModel?> GetReportByIdAsync(int id)
        {
            var response = await _client.GetAsync($"api/admin/reports/{id}");

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();

            return JsonHelper.Deserialize<AdminReportViewModel>(json);
        }

        public async Task<bool> UpdateReportStatusAsync(int id, int status)
        {
            var payload = new { Status = status };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PatchAsync($"api/admin/reports/{id}/status", content);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> AddMessageAsync(int reportId, string message)
        {
            var payload = new { Message = message };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"api/admin/reports/{reportId}/messages", content);

            return response.IsSuccessStatusCode;
        }

        public async Task<List<AdminReportMessageViewModel>> GetMessagesAsync(int reportId)
        {
            var response = await _client.GetAsync($"api/admin/reports/{reportId}/messages");

            if (!response.IsSuccessStatusCode)
                return new List<AdminReportMessageViewModel>();

            var json = await response.Content.ReadAsStringAsync();

            return JsonHelper.Deserialize<List<AdminReportMessageViewModel>>(json) ?? new List<AdminReportMessageViewModel>();
        }
    }
}
