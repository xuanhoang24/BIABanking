using BankingSystemMVC.Areas.Admin.Models;
using BankingSystemMVC.Areas.Admin.Services.Interfaces.Auth;
using BankingSystemMVC.Models.Dtos.Auth;
using System.Text;
using System.Text.Json;

namespace BankingSystemMVC.Areas.Admin.Services.Implementations.Auth
{
    public class AdminAuthApiClient : IAdminAuthApiClient
    {
        private readonly HttpClient _client;

        public AdminAuthApiClient(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("AdminApi");
        }

        public async Task<LoginResponseDto?> LoginAsync(AdminLoginViewModel model)
        {
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("api/admin/auth/login", content);
            if (!response.IsSuccessStatusCode)
                return null;

            var body = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<LoginResponseDto>(body, options);
        }

        public async Task<bool> ChangePasswordAsync(string email, string currentPassword, string newPassword)
        {
            var payload = new
            {
                Email = email,
                CurrentPassword = currentPassword,
                NewPassword = newPassword
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("api/admin/auth/change-password", content);
            return response.IsSuccessStatusCode;
        }
    }

}
