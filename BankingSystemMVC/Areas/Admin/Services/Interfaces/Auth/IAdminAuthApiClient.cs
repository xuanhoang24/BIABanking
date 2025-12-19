using BankingSystemMVC.Areas.Admin.Models;
using BankingSystemMVC.Models.Dtos.Auth;

namespace BankingSystemMVC.Areas.Admin.Services.Interfaces.Auth
{
    public interface IAdminAuthApiClient
    {
        Task<LoginResponseDto?> LoginAsync(AdminLoginViewModel model);
    }
}
