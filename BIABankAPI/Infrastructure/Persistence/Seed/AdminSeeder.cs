using BankingSystemAPI.Infrastructure.Persistence;
using BankingSystemAPI.Infrastructure.Security.Interfaces;
using BankingSystemAPI.Domain.Entities.Users.Admin;
using BankingSystemAPI.Domain.Entities.Users.Roles;
using Microsoft.EntityFrameworkCore;

namespace BankingSystemAPI.Infrastructure.Persistence.Seed
{
    public static class AdminSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

            if (await context.AdminUsers.AnyAsync())
                return;

            var superAdminRole = await context.Roles.SingleAsync(r => r.Name == "SuperAdmin");
            var managerRole = await context.Roles.SingleAsync(r => r.Name == "Manager");
            var reviewerRole = await context.Roles.SingleAsync(r => r.Name == "KycReviewer");

            passwordHasher.CreateHash(
                "IDontKnowWhyISetThisPasswordSoLong*",
                out var hash,
                out var salt
            );

            var superAdmin = new AdminUser
            {
                FirstName = "Super",
                LastName = "Admin",
                Email = "admin@biabank.com",
                PasswordHash = hash,
                PasswordSalt = salt,
                IsActive = true,
                UserRoles =
                {
                    new UserRole { RoleId = superAdminRole.Id }
                }
            };

            var manager = new AdminUser
            {
                FirstName = "System",
                LastName = "Manager",
                Email = "manager@biabank.com",
                PasswordHash = hash,
                PasswordSalt = salt,
                IsActive = true,
                UserRoles =
                {
                    new UserRole { RoleId = managerRole.Id }
                }
            };

            var reviewer = new AdminUser
            {
                FirstName = "KYC",
                LastName = "Reviewer",
                Email = "kyc@biabank.com",
                PasswordHash = hash,
                PasswordSalt = salt,
                IsActive = true,
                UserRoles =
                {
                    new UserRole { RoleId = reviewerRole.Id }
                }
            };

            context.AdminUsers.AddRange(
                superAdmin,
                manager,
                reviewer
            );

            await context.SaveChangesAsync();
        }
    }
}
