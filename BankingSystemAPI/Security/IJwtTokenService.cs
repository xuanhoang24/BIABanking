using BankingSystemAPI.Models.Users;

namespace BankingSystemAPI.Security
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
    }
}
