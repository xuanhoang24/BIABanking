using BankingSystemMVC.Areas.Admin.Models;

namespace BankingSystemMVC.Areas.Admin.Services.Interfaces
{
    public interface IAdminUserApiClient
    {
        Task<List<AdminRoleViewModel>?> GetRolesAsync(); 
        Task<List<AdminUserListViewModel>?> GetAdminUsersAsync();
        Task<bool> CreateAdminUserAsync(AdminUserCreateViewModel model);
    }
}
