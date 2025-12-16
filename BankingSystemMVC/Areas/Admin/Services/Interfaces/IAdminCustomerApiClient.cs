using BankingSystemMVC.Areas.Admin.Models;

namespace BankingSystemMVC.Areas.Admin.Services.Interfaces
{
    public interface IAdminCustomerApiClient
    {
        Task<List<CustomerListViewModel>?> GetAllCustomersAsync();
        Task<CustomerDetailViewModel?> GetCustomerByIdAsync(int customerId);
        Task<List<AccountSummaryViewModel>?> GetCustomerAccountsAsync(int customerId);
        Task<List<TransactionSummaryViewModel>?> GetCustomerTransactionsAsync(int customerId, int limit = 50);
        Task<bool> UpdateCustomerStatusAsync(int customerId, string status);
        Task<List<AccountListViewModel>?> GetAllAccountsAsync();
        Task<List<TransactionListViewModel>?> GetAllTransactionsAsync(int limit = 100);
    }
}
