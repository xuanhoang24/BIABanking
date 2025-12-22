using BankingSystemMVC.Areas.Admin.Models;
using BankingSystemMVC.Areas.Admin.Models.ViewModels.Kyc;
using BankingSystemMVC.Areas.Admin.Services.Interfaces.Kyc;
using BankingSystemMVC.Infrastructure.Json;
using System.Text.Json;

namespace BankingSystemMVC.Areas.Admin.Services.Implementations.Kyc
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

            return JsonHelper.Deserialize<List<AdminKycPendingViewModel>>(json) ?? new();
        }

        public async Task<AdminKycReviewViewModel?> GetForReviewAsync(int id)
        {
            var response = await _client.GetAsync($"api/admin/kyc/{id}");
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();

            return JsonHelper.Deserialize<AdminKycReviewViewModel>(json);
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

        public async Task MarkUnderReviewAsync(int id)
        {
            await _client.PostAsync($"api/admin/kyc/{id}/under-review", null);
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
