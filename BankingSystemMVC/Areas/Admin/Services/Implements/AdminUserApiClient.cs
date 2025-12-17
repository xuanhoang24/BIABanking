using BankingSystemMVC.Areas.Admin.Models;
using BankingSystemMVC.Areas.Admin.Services.Interfaces;
using System.Text;
using System.Text.Json;

namespace BankingSystemMVC.Areas.Admin.Services.Implements
{
    public class AdminUserApiClient : IAdminUserApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public AdminUserApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<List<AdminRoleViewModel>?> GetRolesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/admin/users/roles");

                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<AdminRoleViewModel>>(json, _jsonOptions);
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> CreateAdminUserAsync(AdminUserCreateViewModel model)
        {
            try
            {
                var payload = new
                {
                    model.FirstName,
                    model.LastName,
                    model.Email,
                    model.Password,
                    model.RoleId
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync("api/admin/users", content);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
