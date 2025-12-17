using BankingSystemAPI.Models.Users.Admin;
using BankingSystemAPI.Models.Users.Roles;
using BankingSystemAPI.Security.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankingSystemAPI.DataLayer.Seed
{
    public static class AdminSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

            await context.Database.MigrateAsync();

            if (await context.AdminUsers.AnyAsync())
                return;

            var superAdminRole = await context.Roles.SingleAsync(r => r.Name == "SuperAdmin");
            var managerRole = await context.Roles.SingleAsync(r => r.Name == "Manager");
            var reviewerRole = await context.Roles.SingleAsync(r => r.Name == "KycReviewer");

            passwordHasher.CreateHash(
                "Admin@123",
                out var hash,
                out var salt
            );

            var superAdmin = new AdminUser
            {
                FirstName = "Super",
                LastName = "Admin",
                Email = "admin@bank.com",
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
                Email = "manager@bank.com",
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
                Email = "kyc@bank.com",
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
