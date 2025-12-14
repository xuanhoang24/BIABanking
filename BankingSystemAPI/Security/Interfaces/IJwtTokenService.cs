using BankingSystemAPI.Models.Users;
using BankingSystemAPI.Models.Users.Admin;

namespace BankingSystemAPI.Security.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateUserToken(User user);
        string GenerateAdminToken(AdminUser admin);

    }
}
