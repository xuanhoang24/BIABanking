using BankingSystemMVC.Models.Customers;

namespace BankingSystemMVC.Services.Interfaces
{
    public interface ICustomerApiClient
    {
        Task<CustomerMeViewModel?> GetMeAsync();
    }
}
