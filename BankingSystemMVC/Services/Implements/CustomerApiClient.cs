using BankingSystemMVC.Models.Common;
using BankingSystemMVC.Models.Customers;
using BankingSystemMVC.Models.Kyc;
using BankingSystemMVC.Services.Interfaces;
using System.Net.Http.Headers;
using System.Text.Json;

namespace BankingSystemMVC.Services.Implements
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

            return JsonSerializer.Deserialize<CustomerMeViewModel>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
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

            return JsonSerializer.Deserialize<KycSubmissionViewModel>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
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
    }
}
