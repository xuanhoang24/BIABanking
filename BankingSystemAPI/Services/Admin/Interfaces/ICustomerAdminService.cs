using BankingSystemAPI.Models.DTOs.Admin;

namespace BankingSystemAPI.Services.Admin.Interfaces
{
    public interface ICustomerAdminService
    {
        Task<List<CustomerListDto>> GetAllCustomersAsync();
        Task<CustomerDetailDto?> GetCustomerByIdAsync(int customerId);
        Task<List<AccountSummaryDto>> GetCustomerAccountsAsync(int customerId);
        Task<List<TransactionSummaryDto>> GetCustomerTransactionsAsync(int customerId, int limit = 50);
        Task<bool> UpdateCustomerStatusAsync(int customerId, string status);
        Task<List<AccountListDto>> GetAllAccountsAsync();
        Task<List<TransactionListDto>> GetAllTransactionsAsync(int limit = 100);
    }
}
