using BankingSystemMVC.Models.Accounts;

namespace BankingSystemMVC.Services.Interfaces
{
    public interface IAccountApiClient
    {
        Task<List<AccountSummaryViewModel>> GetMyAccountsAsync();
        Task<bool> CreateAccountAsync(CreateAccountViewModel model);
        Task<AccountDetailViewModel?> GetAccountDetailAsync(int accountId);
        Task<bool> DepositAsync(int accountId, long amountInCents, string? description);
    }
}
