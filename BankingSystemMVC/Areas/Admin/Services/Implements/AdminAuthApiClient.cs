using BankingSystemMVC.Areas.Admin.Models;
using BankingSystemMVC.Areas.Admin.Services.Interfaces;
using BankingSystemMVC.Models.Auth;
using System.Text;
using System.Text.Json;

namespace BankingSystemMVC.Areas.Admin.Services.Implements
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
            return JsonSerializer.Deserialize<LoginResponseDto>(
                body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
        }
    }

}
