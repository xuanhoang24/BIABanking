using BankingSystemMVC.Areas.Admin.Models;
using BankingSystemMVC.Models.Auth;

namespace BankingSystemMVC.Areas.Admin.Services.Interfaces
{
    public interface IAdminAuthApiClient
    {
        Task<LoginResponseDto?> LoginAsync(AdminLoginViewModel model);
    }
}
