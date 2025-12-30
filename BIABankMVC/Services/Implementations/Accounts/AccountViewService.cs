using BankingSystemMVC.Infrastructure.Time;
using BankingSystemMVC.Models.ViewModels.Accounts;
using BankingSystemMVC.Services.Interfaces.Accounts;

namespace BankingSystemMVC.Services.Implementations.Accounts
{
    public class AccountViewService : IAccountViewService
    {
        private readonly IAccountApiClient _accountApi;

        public AccountViewService(IAccountApiClient accountApi)
        {
            _accountApi = accountApi;
        }

        public async Task<AccountDetailViewModel?> GetAccountDetailAsync(int accountId, string timeZoneId, TransactionFilterViewModel? filter = null)
        {
            var apiResult = await _accountApi.GetAccountDetailAsync(accountId, filter);
            if (apiResult == null)
                return null;

            apiResult.RecentTransactions = apiResult.RecentTransactions
                .Select(t => new AccountTransactionViewModel
                {
                    Date = t.Date,
                    LocalTime = TimeZoneHelper.ConvertUtcToLocal(t.Date, timeZoneId),
                    Type = t.Type,
                    Description = t.Description,
                    Amount = t.Amount,
                    PostBalance = t.PostBalance,
                    Status = t.Status,
                    Reference = t.Reference
                })
                .ToList();

            apiResult.Filter = filter ?? new TransactionFilterViewModel { AccountId = accountId };
            apiResult.Filter.AccountId = accountId;

            return apiResult;
        }

    }
}
