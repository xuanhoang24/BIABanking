using BankingSystemMVC.Models.ViewModels.Accounts;

namespace BankingSystemMVC.Services.Interfaces.Accounts
{
    public interface IAccountViewService
    {
        Task<AccountDetailViewModel?> GetAccountDetailAsync(int accountId, string timeZoneId);
    }
}
