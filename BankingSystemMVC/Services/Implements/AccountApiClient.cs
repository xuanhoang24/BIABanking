using BankingSystemMVC.Models.Accounts;
using BankingSystemMVC.Services.Interfaces;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BankingSystemMVC.Services.Implements
{
    public class AccountApiClient : IAccountApiClient
    {
        private readonly HttpClient _client;

        public AccountApiClient(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("BankingSystemAPI");
        }

        public async Task<List<AccountViewModel>> GetMyAccountsAsync(string token)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("api/accounts");

            if (!response.IsSuccessStatusCode)
                return new List<AccountViewModel>();

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<AccountViewModel>>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? new();
        }

        public async Task<bool> CreateAccountAsync(string token, CreateAccountViewModel model)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("api/accounts", content);
            return response.IsSuccessStatusCode;
        }

    }
}
