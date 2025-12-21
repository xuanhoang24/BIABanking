using BankingSystemAPI.Application.Dtos.Admin;
using BankingSystemAPI.Application.Dtos.Accounts;

namespace BankingSystemAPI.Application.Services.Interfaces.Admin
{
    public interface ICustomerAdminService
    {
        Task<List<CustomerListDto>> GetAllCustomersAsync();
        Task<CustomerDetailDto?> GetCustomerByIdAsync(int customerId);
        Task<List<AccountSummaryDto>> GetCustomerAccountsAsync(int customerId);
        Task<List<TransactionSummaryDto>> GetCustomerTransactionsAsync(int customerId, int limit = 50);
        Task<bool> UpdateCustomerStatusAsync(int customerId, string status);
        Task<bool> ResetCustomerPasswordAsync(int customerId);
        Task<bool> DeleteCustomerAsync(int customerId);
        Task<List<AccountListDto>> GetAllAccountsAsync();
        Task<List<TransactionListDto>> GetAllTransactionsAsync(int limit = 100);
    }
}
