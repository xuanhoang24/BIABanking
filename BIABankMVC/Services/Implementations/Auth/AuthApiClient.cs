using BankingSystemMVC.Models.Dtos.Auth;
using BankingSystemMVC.Models.ViewModels.Auth;
using BankingSystemMVC.Services.Interfaces.Auth;
using System.Text;
using System.Text.Json;

namespace BankingSystemMVC.Services.Implementations.Auth
{
    public class AuthApiClient : IAuthApiClient
    {
        private readonly HttpClient _client;

        public AuthApiClient(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("CustomerApi");
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginViewModel model)
        {
            var json = JsonSerializer.Serialize(model);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("api/auth/login", content);
            
            if (!response.IsSuccessStatusCode)
            {
                // Try to get error message from response
                var errorBody = await response.Content.ReadAsStringAsync();
                try
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var errorResponse = JsonSerializer.Deserialize<Dictionary<string, string>>(errorBody, options);
                    var errorMessage = errorResponse?.GetValueOrDefault("error");
                    
                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        throw new InvalidOperationException(errorMessage);
                    }
                }
                catch (JsonException)
                {
                    // If JSON parsing fails, just return null
                }
                
                return null;
            }

            var body = await response.Content.ReadAsStringAsync();
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<LoginResponseDto>(body, jsonOptions);
        }

        public async Task<(bool Success, string? ErrorMessage)> RegisterAsync(RegisterViewModel model)
        {
            var json = JsonSerializer.Serialize(model);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("api/auth/register", content);
            
            if (response.IsSuccessStatusCode)
                return (true, null);

            var errorBody = await response.Content.ReadAsStringAsync();
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var errorResponse = JsonSerializer.Deserialize<Dictionary<string, string>>(errorBody, options);
                return (false, errorResponse?.GetValueOrDefault("error") ?? "Registration failed");
            }
            catch
            {
                return (false, "Registration failed");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> VerifyEmailAsync(string token)
        {
            var response = await _client.GetAsync($"api/auth/verify-email?token={Uri.EscapeDataString(token)}");

            if (response.IsSuccessStatusCode)
                return (true, null);

            var errorBody = await response.Content.ReadAsStringAsync();
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var errorResponse = JsonSerializer.Deserialize<Dictionary<string, string>>(errorBody, options);
                return (false, errorResponse?.GetValueOrDefault("error") ?? "Verification failed");
            }
            catch
            {
                return (false, "Verification failed");
            }
        }
    }
}
