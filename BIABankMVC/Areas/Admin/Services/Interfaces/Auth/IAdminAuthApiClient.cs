using BankingSystemMVC.Areas.Admin.Models;
using BankingSystemMVC.Areas.Admin.Models.ViewModels.Auth;
using BankingSystemMVC.Models.Dtos.Auth;

namespace BankingSystemMVC.Areas.Admin.Services.Interfaces.Auth
{
    public interface IAdminAuthApiClient
    {
        Task<LoginResponseDto?> LoginAsync(AdminLoginViewModel model);
        Task<bool> ChangePasswordAsync(string email, string currentPassword, string newPassword);
    }
}
