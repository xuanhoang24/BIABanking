namespace BankingSystemAPI.Services.Customer.Interfaces
{
    public interface ITransactionService
    {
        Task<string> DepositAsync(int customerId, int accountId, decimal amount, string? description);
        Task<string> WithdrawAsync(int customerId, int accountId, decimal amount, string? description);
        Task<string> TransferAsync(int customerId, int fromAccountId, string toAccountNumber, decimal amount, string? description);
        Task<string> PaymentAsync(int customerId, int accountId, decimal amount, string merchant);
    }
}
