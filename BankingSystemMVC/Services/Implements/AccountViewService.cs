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

        public async Task<AccountDetailViewModel?> GetAccountDetailAsync( int accountId, string timeZoneId)
        {
            var apiResult = await _accountApi.GetAccountDetailAsync(accountId);
            if (apiResult == null)
                return null;

            apiResult.RecentDeposits = apiResult.RecentDeposits
                .Select(d => new RecentDepositViewModel
                {
                    Amount = d.Amount,
                    Description = d.Description,
                    LocalTime = TimeZoneHelper.ConvertUtcToLocal(
                        d.Date,
                        timeZoneId
                    )
                })
                .ToList();

            return apiResult;
        }
    }
}
