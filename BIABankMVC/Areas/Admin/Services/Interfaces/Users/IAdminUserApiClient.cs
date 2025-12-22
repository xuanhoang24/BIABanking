using BankingSystemMVC.Areas.Admin.Models;
using BankingSystemMVC.Areas.Admin.Models.ViewModels.Users;

namespace BankingSystemMVC.Areas.Admin.Services.Interfaces.Users
{
    public interface IAdminUserApiClient
    {
        Task<List<AdminRoleViewModel>?> GetRolesAsync(); 
        Task<List<AdminUserListViewModel>?> GetAdminUsersAsync();
        Task<AdminUserDetailViewModel?> GetAdminUserByIdAsync(int id);
        Task<bool> CreateAdminUserAsync(AdminUserCreateViewModel model);
        Task<bool> ResetAdminPasswordAsync(int id);
        Task<bool> ToggleAdminStatusAsync(int id);
    }
}
