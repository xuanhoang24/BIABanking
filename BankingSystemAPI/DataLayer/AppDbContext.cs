using BankingSystemAPI.Models.Accounts;
using BankingSystemAPI.Models.Users.Admin;
using BankingSystemAPI.Models.Users.Customers;
using BankingSystemAPI.Models.Users.Roles;
using Microsoft.EntityFrameworkCore;

namespace BankingSystemAPI.DataLayer
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Users & Roles Entities
        public DbSet<Customer> Customers { get; set; }
        public DbSet<AdminUser> AdminUsers { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Permission> Permissions { get; set; }

        // Banking Entities
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<LedgerEntry> LedgerEntries { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        // KYC Entities
        public DbSet<KYCDocument> KYCDocuments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Customer Configuration
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.PasswordHash).IsRequired();
            });

            // AdminUser Configuration
            modelBuilder.Entity<AdminUser>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.PasswordHash).IsRequired();
            });

            // Account Configuration
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasIndex(e => e.AccountNumber).IsUnique();
                entity.Property(e => e.AccountNumber).IsRequired();

                entity.HasOne(a => a.Customer)
                      .WithMany(c => c.Accounts)
                      .HasForeignKey(a => a.CustomerId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany()
                .HasForeignKey(ur => ur.RoleId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.AdminUser)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.AdminUserId);

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId);

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany()
                .HasForeignKey(rp => rp.PermissionId);

            // Transaction Configuration
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasIndex(e => e.TransactionReference).IsUnique();
                entity.Property(e => e.TransactionReference).IsRequired();

                entity.HasOne(t => t.FromCustomer)
                      .WithMany(c => c.SentTransactions)
                      .HasForeignKey(t => t.FromCustomerId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.ToCustomer)
                      .WithMany(c => c.ReceivedTransactions)
                      .HasForeignKey(t => t.ToCustomerId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.FromAccount)
                      .WithMany(a => a.DebitTransactions)
                      .HasForeignKey(t => t.FromAccountId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.ToAccount)
                      .WithMany(a => a.CreditTransactions)
                      .HasForeignKey(t => t.ToAccountId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // LedgerEntry Configuration
            modelBuilder.Entity<LedgerEntry>(entity =>
            {
                entity.HasOne(le => le.Transaction)
                      .WithMany(t => t.LedgerEntries)
                      .HasForeignKey(le => le.TransactionId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(le => le.Account)
                      .WithMany(a => a.LedgerEntries)
                      .HasForeignKey(le => le.AccountId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.AccountId, e.CreatedAt });
            });

            // AuditLog Configuration
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasIndex(e => new { e.EntityType, e.EntityId });
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => e.CustomerId);
            });

            // KYCDocument Configuration
            modelBuilder.Entity<KYCDocument>(entity =>
            {
                entity.HasOne(k => k.Customer)
                      .WithMany()
                      .HasForeignKey(k => k.CustomerId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(k => k.ReviewedByAdmin)
                      .WithMany(a => a.ReviewedDocuments)
                      .HasForeignKey(k => k.ReviewedByAdminId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => e.CustomerId)
                      .IsUnique();

                entity.HasIndex(e => e.Status);
            });


        }
    }
}
