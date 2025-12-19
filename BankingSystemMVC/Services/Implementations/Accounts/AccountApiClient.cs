using BankingSystemMVC.Infrastructure.Json;
using BankingSystemMVC.Models.ViewModels.Accounts;
using BankingSystemMVC.Services.Interfaces.Accounts;
using System.Text;
using System.Text.Json;

namespace BankingSystemMVC.Services.Implementations.Accounts
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

            return JsonHelper.Deserialize<List<AccountSummaryViewModel>>(json) ?? new();
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateAccountAsync(CreateAccountViewModel model)
        {
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("api/accounts", content);
            
            if (response.IsSuccessStatusCode)
                return (true, null);

            try
            {
                var errorJson = await response.Content.ReadAsStringAsync();
                var errorObj = JsonSerializer.Deserialize<JsonElement>(errorJson);
                
                if (errorObj.TryGetProperty("message", out var messageElement))
                {
                    return (false, messageElement.GetString());
                }
            }
            catch { }

            return (false, "Failed to create account. Please try again.");
        }

        public async Task<AccountDetailViewModel?> GetAccountDetailAsync(int accountId)
        {
            var response = await _client.GetAsync($"api/accounts/{accountId}");

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();

            return JsonHelper.Deserialize<AccountDetailViewModel>(json);
        }
    }
}