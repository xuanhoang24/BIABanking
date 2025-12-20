using BankingSystemAPI.Infrastructure.Persistence;
using BankingSystemAPI.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;

namespace BankingSystemAPI.Configuration
{
    public static class DatabaseExtensions
    {
        public static async Task InitializeDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await db.Database.MigrateAsync();

            await RoleSeeder.SeedAsync(db);
            await PermissionSeeder.SeedAsync(db);
            await AdminSeeder.SeedAsync(scope.ServiceProvider);
        }
    }
}
