using BankingSystemMVC.Models.ViewModels.Accounts;

namespace BankingSystemMVC.Services.Interfaces.Accounts
{
    public interface IAccountApiClient
    {
        Task<List<AccountSummaryViewModel>> GetMyAccountsAsync();
        Task<(bool Success, string? ErrorMessage)> CreateAccountAsync(CreateAccountViewModel model);
        Task<AccountDetailViewModel?> GetAccountDetailAsync(int accountId);
    }
}
