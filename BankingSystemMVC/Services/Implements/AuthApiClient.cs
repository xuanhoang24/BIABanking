using BankingSystemMVC.Models.Auth;
using BankingSystemMVC.Services.Interfaces;
using System.Text;
using System.Text.Json;

namespace BankingSystemMVC.Services.Implements
{
    public class AuthApiClient : IAuthApiClient
    {
        private readonly HttpClient _client;

        public AuthApiClient(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("BankingSystemAPI");
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginViewModel model)
        {
            var json = JsonSerializer.Serialize(model);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("api/auth/login", content);
            if (!response.IsSuccessStatusCode)
                return null;

            var body = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<LoginResponseDto>(
                body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
        }

        public async Task<bool> RegisterAsync(RegisterViewModel model)
        {
            var json = JsonSerializer.Serialize(model);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("api/auth/register", content);
            return response.IsSuccessStatusCode;
        }
    }
}
