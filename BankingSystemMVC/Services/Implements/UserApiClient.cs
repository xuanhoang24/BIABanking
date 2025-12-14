using BankingSystemMVC.Models.Users;
using BankingSystemMVC.Services.Interfaces;
using System.Net.Http.Headers;
using System.Text.Json;

namespace BankingSystemMVC.Services.Implements
{
    public class UserApiClient : IUserApiClient
    {
        private readonly HttpClient _client;

        public UserApiClient(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("BankingSystemAPI");
        }

        public async Task<UserMeViewModel?> GetMeAsync(string accessToken)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _client.GetAsync("api/users/me");

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<UserMeViewModel>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
        }
    }
}
