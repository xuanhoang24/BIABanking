using BankingSystemMVC.Models.Reports;
using BankingSystemMVC.Services.Interfaces;
using System.Text;
using System.Text.Json;

namespace BankingSystemMVC.Services.Implements
{
    public class ReportApiClient : IReportApiClient
    {
        private readonly HttpClient _client;

        public ReportApiClient(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("CustomerApi");
        }

        public async Task<(bool Success, string? Error)> CreateReportAsync(CreateReportViewModel model)
        {
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("api/customer/reports", content);

            if (response.IsSuccessStatusCode)
                return (true, null);

            var error = await response.Content.ReadAsStringAsync();
            return (false, error);
        }

        public async Task<List<ReportViewModel>> GetMyReportsAsync()
        {
            var response = await _client.GetAsync("api/customer/reports");

            if (!response.IsSuccessStatusCode)
                return new List<ReportViewModel>();

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<ReportViewModel>>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? new List<ReportViewModel>();
        }

        public async Task<ReportViewModel?> GetReportByIdAsync(int id)
        {
            var response = await _client.GetAsync($"api/customer/reports/{id}");

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ReportViewModel>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
        }
    }
}
