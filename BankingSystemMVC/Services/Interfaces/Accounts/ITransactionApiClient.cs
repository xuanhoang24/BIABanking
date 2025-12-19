using BankingSystemMVC.Models.ViewModels.Accounts.Transactions;

namespace BankingSystemMVC.Services.Interfaces.Accounts
{
    public interface ITransactionApiClient
    {
        Task<string> DepositAsync(DepositViewModel model);
        Task<string> WithdrawAsync(WithdrawViewModel model);
        Task<string> TransferAsync(TransferViewModel model);
        Task<string> PaymentAsync(PaymentViewModel model);
    }
}
