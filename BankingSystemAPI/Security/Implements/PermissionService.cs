using BankingSystemAPI.DataLayer;
using BankingSystemAPI.Security.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankingSystemAPI.Security.Implements
{
    public class PermissionService : IPermissionService
    {
        private readonly AppDbContext _context;

        public PermissionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<string>> GetPermissionsAsync(int adminUserId)
        {
            return await _context.UserRoles
                .Where(ur => ur.AdminUserId == adminUserId)
                .SelectMany(ur => ur.Role!.RolePermissions)
                .Select(rp => rp.Permission!.Code)
                .Distinct()
                .ToListAsync();
        }
    }
}
