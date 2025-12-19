using BankingSystemAPI.Application.Dtos.Customers;

namespace BankingSystemAPI.Application.Services.Interfaces.Customer
{
    public interface ICustomerService
    {
        Task<BankingSystemAPI.Domain.Entities.Users.Customers.Customer> RegisterCustomerAsync(
            string firstName,
            string lastName,
            string email,
            string password,
            string phoneNumber,
            DateTime dateOfBirth,
            string address
        );

        Task<BankingSystemAPI.Domain.Entities.Users.Customers.Customer?> AuthenticateCustomerAsync( string email, string password);
        Task<CustomerMeDto?> GetMeAsync(int customerId);
    }
}
