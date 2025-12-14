using BankingSystemMVC.Models.Accounts;

namespace BankingSystemMVC.Services.Interfaces
{
    public interface IAccountApiClient
    {
        Task<List<AccountViewModel>> GetMyAccountsAsync(string token);
    }
}
