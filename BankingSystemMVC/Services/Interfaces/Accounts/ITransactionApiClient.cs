using BankingSystemMVC.Models.ViewModels.Accounts.Transactions;

namespace BankingSystemMVC.Services.Interfaces.Accounts
{
    public interface ITransactionApiClient
    {
        Task<(bool Success, string? Reference, string? ErrorMessage)> DepositAsync(DepositViewModel model);
        Task<(bool Success, string? Reference, string? ErrorMessage)> WithdrawAsync(WithdrawViewModel model);
        Task<(bool Success, string? Reference, string? ErrorMessage)> TransferAsync(TransferViewModel model);
        Task<(bool Success, string? Reference, string? ErrorMessage)> PaymentAsync(PaymentViewModel model);
    }
}
