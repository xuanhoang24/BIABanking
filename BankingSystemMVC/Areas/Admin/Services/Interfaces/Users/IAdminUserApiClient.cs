using BankingSystemMVC.Areas.Admin.Models;
using BankingSystemMVC.Areas.Admin.Models.ViewModels.Users;

namespace BankingSystemMVC.Areas.Admin.Services.Interfaces.Users
{
    public interface IAdminUserApiClient
    {
        Task<List<AdminRoleViewModel>?> GetRolesAsync(); 
        Task<List<AdminUserListViewModel>?> GetAdminUsersAsync();
        Task<bool> CreateAdminUserAsync(AdminUserCreateViewModel model);
    }
}
