using BankingSystemAPI.Models.Users.Admin;
using BankingSystemAPI.Models.Users.Customers;

namespace BankingSystemAPI.Security.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateCustomerToken(Customer customer);
        string GenerateAdminToken(AdminUser admin);

    }
}
