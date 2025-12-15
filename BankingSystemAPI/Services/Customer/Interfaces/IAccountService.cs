using BankingSystemAPI.Models;

namespace BankingSystemAPI.Services.Customer.Interfaces
{
    public interface IAccountService
    {
        Task<Account?> DepositAsync(int accountId, int customerId, long amountInCents, string? description);
    }
}
