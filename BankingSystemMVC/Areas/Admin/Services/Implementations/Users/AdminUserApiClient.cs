using BankingSystemMVC.Areas.Admin.Models;
using BankingSystemMVC.Areas.Admin.Models.ViewModels.Users;
using BankingSystemMVC.Areas.Admin.Services.Interfaces.Users;
using BankingSystemMVC.Infrastructure.Json;
using System.Text;
using System.Text.Json;

namespace BankingSystemMVC.Areas.Admin.Services.Implementations.Users
{
    public class AdminUserApiClient : IAdminUserApiClient
    {
        private readonly HttpClient _httpClient;

        public AdminUserApiClient(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("AdminApi");
        }

        public async Task<List<AdminRoleViewModel>?> GetRolesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/admin/users/roles");

                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                return JsonHelper.Deserialize<List<AdminRoleViewModel>>(json);
            }
            catch
            {
                return null;
            }
        }
        public async Task<List<AdminUserListViewModel>?> GetAdminUsersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/admin/users");

                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                return JsonHelper.Deserialize<List<AdminUserListViewModel>>(json);
            }
            catch
            {
                return null;
            }
        }

        public async Task<AdminUserDetailViewModel?> GetAdminUserByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/admin/users/{id}");

                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                return JsonHelper.Deserialize<AdminUserDetailViewModel>(json);
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

        public async Task<bool> ResetAdminPasswordAsync(int id)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api/admin/users/{id}/reset-password", null);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ToggleAdminStatusAsync(int id)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api/admin/users/{id}/toggle-status", null);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
