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

        public async Task<AccountDetailViewModel?> GetAccountDetailAsync(
            int accountId,
            string timeZoneId)
        {
            var apiResult = await _accountApi.GetAccountDetailAsync(accountId);
            if (apiResult == null)
                return null;

            apiResult.RecentTransactions = apiResult.RecentTransactions
                .Select(t => new AccountTransactionViewModel
                {
                    Amount = t.Amount,
                    Description = t.Description,
                    Status = t.Status,
                    Type = t.Type,
                    LocalTime = TimeZoneHelper.ConvertUtcToLocal(
                        t.Date,
                        timeZoneId
                    )
                })
                .ToList();

            return apiResult;
        }

    }
}
