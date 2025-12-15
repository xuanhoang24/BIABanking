using BankingSystemMVC.Models.Accounts;
using BankingSystemMVC.Services.Interfaces;
using System.Text;
using System.Text.Json;

namespace BankingSystemMVC.Services.Implements
{
    public class AccountApiClient : IAccountApiClient
    {
        private readonly HttpClient _client;

        public AccountApiClient(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("CustomerApi");
        }

        public async Task<List<AccountSummaryViewModel>> GetMyAccountsAsync()
        {
            var response = await _client.GetAsync("api/accounts");

            if (!response.IsSuccessStatusCode)
                return new();

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<AccountSummaryViewModel>>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? new();
        }

        public async Task<bool> CreateAccountAsync(CreateAccountViewModel model)
        {
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("api/accounts", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<AccountDetailViewModel?> GetAccountDetailAsync(int accountId)
        {
            var response = await _client.GetAsync($"api/accounts/{accountId}");

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<AccountDetailViewModel>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
        }
    }
}