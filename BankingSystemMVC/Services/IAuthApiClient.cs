using BankingSystemMVC.Models.Auth;

namespace BankingSystemMVC.Services
{
    public interface IAuthApiClient
    {
        Task<LoginResponseDto?> LoginAsync(LoginViewModel model);
        Task<bool> RegisterAsync(RegisterViewModel model);
    }
}
