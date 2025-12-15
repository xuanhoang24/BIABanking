namespace BankingSystemAPI.Services.Customer.Interfaces
{
    public interface ITransactionService
    {
        Task DepositAsync(int customerId, int accountId, decimal amount, string? description);
        Task WithdrawAsync(int customerId, int accountId, decimal amount, string? description);
        Task TransferAsync(int customerId, int fromAccountId, string toAccountNumber, decimal amount, string? description);
        Task PaymentAsync(int customerId, int accountId, decimal amount, string merchant);
    }
}
