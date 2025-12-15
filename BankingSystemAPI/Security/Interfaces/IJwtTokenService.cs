using BankingSystemAPI.Models.Users;
using BankingSystemAPI.Models.Users.Admin;

namespace BankingSystemAPI.Security.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateCustomerToken(Customer customer);
        string GenerateAdminToken(AdminUser admin);

    }
}
