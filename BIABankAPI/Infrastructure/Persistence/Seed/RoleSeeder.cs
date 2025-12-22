using BankingSystemAPI.Infrastructure.Persistence;
using BankingSystemAPI.Domain.Entities.Users.Roles;
using Microsoft.EntityFrameworkCore;

namespace BankingSystemAPI.Infrastructure.Persistence.Seed
{
    public class RoleSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (await context.Roles.AnyAsync())
                return;

            context.Roles.AddRange(
                new Role { Name = "SuperAdmin", Description = "Full system access" },
                new Role { Name = "Manager", Description = "Manage users and KYC" },
                new Role { Name = "KycReviewer", Description = "Review KYC only" }
            );

            await context.SaveChangesAsync();
        }
    }
}
