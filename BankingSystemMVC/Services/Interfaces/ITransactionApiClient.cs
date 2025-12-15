using BankingSystemMVC.Models.Accounts.Transactions;

namespace BankingSystemMVC.Services.Interfaces
{
    public interface ITransactionApiClient
    {
        Task<bool> DepositAsync(DepositViewModel model);
        Task<bool> WithdrawAsync(WithdrawViewModel model);
        Task<bool> TransferAsync(TransferViewModel model);
        Task<bool> PaymentAsync(PaymentViewModel model);
    }
}
