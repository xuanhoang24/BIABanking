using BankingSystemAPI.Models.Users.Admin;
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

            passwordHasher.CreateHash(
                "Admin@123",
                out var hash,
                out var salt
            );

            var admin = new AdminUser
            {
                FirstName = "Super",
                LastName = "Admin",
                Email = "admin@bank.com",
                PasswordHash = hash,
                PasswordSalt = salt,
                Role = AdminRole.SuperAdmin,
                IsActive = true
            };

            context.AdminUsers.Add(admin);
            await context.SaveChangesAsync();
        }
    }
}
