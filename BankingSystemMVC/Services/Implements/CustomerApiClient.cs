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

        public async Task<bool> UploadKycAsync(UploadKycViewModel model)
        {
            using var form = new MultipartFormDataContent();

            form.Add(
                new StringContent(((int)model.DocumentType).ToString()),
                "DocumentType"
            );

            var fileContent = new StreamContent(model.File.OpenReadStream());
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(model.File.ContentType);

            form.Add(fileContent, "File", model.File.FileName);

            var response = await _client.PostAsync("api/kyc/upload", form);

            return response.IsSuccessStatusCode;
        }

        public async Task<List<KycSubmissionViewModel>> GetMyKycSubmissionsAsync()
        {
            var response = await _client.GetAsync("api/kyc/my-documents");

            if (!response.IsSuccessStatusCode)
                return new List<KycSubmissionViewModel>();

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<KycSubmissionViewModel>>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? new List<KycSubmissionViewModel>();
        }
    }
}
