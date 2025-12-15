using BankingSystemMVC.Models.Accounts;

namespace BankingSystemMVC.Services.Interfaces
{
    public interface IAccountViewService
    {
        Task<AccountDetailViewModel?> GetAccountDetailAsync(int accountId, string timeZoneId);
    }
}
