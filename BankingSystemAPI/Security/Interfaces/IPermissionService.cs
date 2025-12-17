using BankingSystemAPI.Models.Users.Admin;

namespace BankingSystemAPI.Security.Interfaces
{
    public interface IPermissionService
    {
        Task<IReadOnlyList<string>> GetPermissionsAsync(int adminUserId);
    }
}
