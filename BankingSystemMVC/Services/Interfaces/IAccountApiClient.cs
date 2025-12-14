using BankingSystemMVC.Models.Accounts;

namespace BankingSystemMVC.Services.Interfaces
{
    public interface IAccountApiClient
    {
        Task<List<AccountSummaryViewModel>> GetMyAccountsAsync(string token);
        Task<bool> CreateAccountAsync(string token, CreateAccountViewModel model);
        Task<AccountDetailViewModel?> GetAccountDetailAsync(int accountId, string token);
    }
}
