using BankingSystemMVC.Models.Accounts.Transactions;

namespace BankingSystemMVC.Services.Interfaces
{
    public interface ITransactionApiClient
    {
        Task<string> DepositAsync(DepositViewModel model);
        Task<string> WithdrawAsync(WithdrawViewModel model);
        Task<string> TransferAsync(TransferViewModel model);
        Task<string> PaymentAsync(PaymentViewModel model);
    }
}
