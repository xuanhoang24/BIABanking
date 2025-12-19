using BankingSystemMVC.Infrastructure.Json;
using BankingSystemMVC.Models.Common;
using BankingSystemMVC.Models.Kyc;
using BankingSystemMVC.Models.ViewModels.Customers;
using BankingSystemMVC.Models.ViewModels.Kyc;
using BankingSystemMVC.Services.Interfaces.Customers;
using System.Net.Http.Headers;

namespace BankingSystemMVC.Services.Implementations.Customers
{
    public class CustomerApiClient : ICustomerApiClient
    {
        private readonly HttpClient _client;

        public CustomerApiClient(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("CustomerApi");
        }

        public async Task<CustomerMeViewModel?> GetMeAsync()
        {
            var response = await _client.GetAsync("api/customers/me");

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();

            return JsonHelper.Deserialize<CustomerMeViewModel>(json);
        }

        public async Task<(bool Success, string? Error)> UploadKycAsync(UploadKycViewModel model)
        {
            using var form = new MultipartFormDataContent();

            form.Add(
                new StringContent(((int)model.DocumentType).ToString()),
                "DocumentType"
            );

            var fileContent = new StreamContent(model.File.OpenReadStream());
            fileContent.Headers.ContentType =
                new MediaTypeHeaderValue(model.File.ContentType);

            form.Add(fileContent, "File", model.File.FileName);

            var response = await _client.PostAsync("api/kyc/upload", form);

            if (response.IsSuccessStatusCode)
                return (true, null);

            var json = await response.Content.ReadAsStringAsync();
            return (false, json);
        }

        public async Task<KycSubmissionViewModel?> GetMyKycAsync()
        {
            var response = await _client.GetAsync("api/kyc/my-document");
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();

            return JsonHelper.Deserialize<KycSubmissionViewModel>(json);
        }
        public async Task<ApiFileResult?> GetMyKycFileAsync()
        {
            var response = await _client.GetAsync("api/kyc/my-document/file");
            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadAsByteArrayAsync();
            var contentType = response.Content.Headers.ContentType?.ToString()
                              ?? "application/octet-stream";

            return new ApiFileResult
            {
                Content = content,
                ContentType = contentType
            };
        }

        public async Task<bool> ChangePasswordAsync(string currentPassword, string newPassword)
        {
            var payload = new
            {
                CurrentPassword = currentPassword,
                NewPassword = newPassword
            };

            var json = System.Text.Json.JsonSerializer.Serialize(payload);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("api/customers/change-password", content);
            return response.IsSuccessStatusCode;
        }
    }
}
