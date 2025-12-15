using BankingSystemAPI.Models.DTOs.Customer;
namespace BankingSystemAPI.Services.Customer.Interfaces
{
    public interface ICustomerService
    {
        Task<BankingSystemAPI.Models.Users.Customer> RegisterCustomerAsync(
            string firstName,
            string lastName,
            string email,
            string password,
            string phoneNumber,
            DateTime dateOfBirth,
            string address
        );

        Task<BankingSystemAPI.Models.Users.Customer?> AuthenticateCustomerAsync( string email, string password);
        Task<CustomerMeDto?> GetMeAsync(int customerId);
    }
}
