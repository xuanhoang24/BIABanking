using BankingSystemMVC.Areas.Admin.Models;
using BankingSystemMVC.Areas.Admin.Services.Interfaces;
using System.Text.Json;

namespace BankingSystemMVC.Areas.Admin.Services.Implements
{
    public class AdminKycApiClient : IAdminKycApiClient
    {
        private readonly HttpClient _client;

        public AdminKycApiClient(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("AdminApi");
        }

        public async Task<List<AdminKycPendingViewModel>> GetPendingAsync()
        {
            var response = await _client.GetAsync("api/admin/kyc/pending");
            if (!response.IsSuccessStatusCode)
                return new();

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<AdminKycPendingViewModel>>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? new();
        }

        public async Task<AdminKycReviewViewModel?> GetForReviewAsync(int id)
        {
            var response = await _client.GetAsync($"api/admin/kyc/{id}");
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<AdminKycReviewViewModel>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
        }

        public async Task<KycFileResult?> GetFileAsync(int id)
        {
            var response = await _client.GetAsync($"api/admin/kyc/{id}/file");
            if (!response.IsSuccessStatusCode)
                return null;

            var bytes = await response.Content.ReadAsByteArrayAsync();
            var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
            var fileName = response.Content.Headers.ContentDisposition?.FileName?.Trim('"') ?? "kyc-file";

            return new KycFileResult
            {
                Bytes = bytes,
                ContentType = contentType,
                FileName = fileName
            };
        }

        public async Task ApproveAsync(int id)
        {
            await _client.PostAsync($"api/admin/kyc/{id}/approve", null);
        }

        public async Task RejectAsync(int id, string reviewNotes)
        {
            var body = JsonContent.Create(new { reviewNotes });
            await _client.PostAsync($"api/admin/kyc/{id}/reject", body);
        }
    }
}
