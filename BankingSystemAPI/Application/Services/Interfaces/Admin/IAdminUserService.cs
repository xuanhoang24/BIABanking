using BankingSystemAPI.Application.Dtos.Admin;
using BankingSystemAPI.Domain.Entities.Users.Admin;
using BankingSystemAPI.Domain.Entities.Users.Roles;

namespace BankingSystemAPI.Application.Services.Interfaces.Admin
{
    public interface IAdminUserService
    {
        Task<AdminUser?> AuthenticateAsync(string email, string password);

        Task<IReadOnlyList<Role>> GetRolesAsync();

        Task<List<AdminUserListDto>> GetAllAdminUsersAsync();

        Task<AdminUser?> CreateAdminUserAsync(
            string firstName,
            string lastName,
            string email,
            string password,
            int roleId);

        Task<bool> UpdatePasswordAsync(string email, string newPassword);
    }
}
