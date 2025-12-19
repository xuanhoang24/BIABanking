using BankingSystemAPI.Domain.Entities.Users.Admin;

namespace BankingSystemAPI.Infrastructure.Security.Interfaces
{
    public interface IPermissionService
    {
        Task<IReadOnlyList<string>> GetPermissionsAsync(int adminUserId);
    }
}
