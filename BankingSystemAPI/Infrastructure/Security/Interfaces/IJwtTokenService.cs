using BankingSystemAPI.Domain.Entities.Users.Admin;
using BankingSystemAPI.Domain.Entities.Users.Customers;

namespace BankingSystemAPI.Infrastructure.Security.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateCustomerToken(Customer customer);
        Task<string> GenerateAdminTokenAsync(AdminUser admin);
    }
}
