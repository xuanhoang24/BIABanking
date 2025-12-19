using BankingSystemMVC.Models.Dtos.Auth;
using BankingSystemMVC.Models.ViewModels.Auth;

namespace BankingSystemMVC.Services.Interfaces.Auth
{
    public interface IAuthApiClient
    {
        Task<LoginResponseDto?> LoginAsync(LoginViewModel model);
        Task<(bool Success, string? ErrorMessage)> RegisterAsync(RegisterViewModel model);
    }
}
