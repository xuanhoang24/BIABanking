using BankingSystemAPI.Models.Users.Roles;
using Microsoft.EntityFrameworkCore;

namespace BankingSystemAPI.DataLayer.Seed
{
    public static class PermissionSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (await context.Permissions.AnyAsync())
                return;

            var permissions = new[]
            {
                new Permission { Code = PermissionCodes.DashboardView, Description = "View dashboard" },
                new Permission { Code = PermissionCodes.CustomerRead, Description = "Read customers" },
                new Permission { Code = PermissionCodes.CustomerManage, Description = "Manage customers" },
                new Permission { Code = PermissionCodes.KycRead, Description = "View KYC" },
                new Permission { Code = PermissionCodes.KycReview, Description = "Review KYC" }
            };

            context.Permissions.AddRange(permissions);
            await context.SaveChangesAsync();

            void grant(string roleName, params string[] codes)
            {
                var role = context.Roles.Single(r => r.Name == roleName);

                foreach (var code in codes)
                {
                    context.RolePermissions.Add(new RolePermission
                    {
                        RoleId = role.Id,
                        PermissionId = permissions.Single(p => p.Code == code).Id
                    });
                }
            }

            grant("SuperAdmin",
                PermissionCodes.DashboardView,
                PermissionCodes.CustomerRead,
                PermissionCodes.CustomerManage,
                PermissionCodes.KycRead,
                PermissionCodes.KycReview
            );

            grant("Manager",
                PermissionCodes.DashboardView,
                PermissionCodes.CustomerRead,
                PermissionCodes.CustomerManage,
                PermissionCodes.KycRead,
                PermissionCodes.KycReview
            );

            grant("KycReviewer",
                PermissionCodes.KycRead,
                PermissionCodes.KycReview
            );

            await context.SaveChangesAsync();
        }
    }
}
