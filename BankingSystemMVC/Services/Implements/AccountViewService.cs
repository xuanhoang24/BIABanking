using BankingSystemMVC.Infrastructure.Time;
using BankingSystemMVC.Models.Accounts;
using BankingSystemMVC.Services.Interfaces;

namespace BankingSystemMVC.Services.Implements
{
    public class AccountViewService : IAccountViewService
    {
        private readonly IAccountApiClient _accountApi;

        public AccountViewService(IAccountApiClient accountApi)
        {
            _accountApi = accountApi;
        }

        public async Task<AccountDetailViewModel?> GetAccountDetailAsync(int accountId, string timeZoneId)
        {
            var apiResult = await _accountApi.GetAccountDetailAsync(accountId);
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

            return apiResult;
        }

    }
}
