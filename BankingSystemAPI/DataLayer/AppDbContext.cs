using BankingSystemAPI.Models;
using BankingSystemAPI.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace BankingSystemAPI.DataLayer
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Users & Roles Entities
        public DbSet<User> Users { get; set; }
        public DbSet<AdminUser> AdminUsers { get; set; }

        // Banking Entities
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<LedgerEntry> LedgerEntries { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        // KYC Entities
        public DbSet<KYCDocument> KYCDocuments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User Configuration
            modelBuilder.Entity<User>(entity =>
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

                entity.HasOne(a => a.User)
                      .WithMany(u => u.Accounts)
                      .HasForeignKey(a => a.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Transaction Configuration
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasIndex(e => e.TransactionReference).IsUnique();
                entity.Property(e => e.TransactionReference).IsRequired();

                entity.HasOne(t => t.FromUser)
                      .WithMany(u => u.SentTransactions)
                      .HasForeignKey(t => t.FromUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.ToUser)
                      .WithMany(u => u.ReceivedTransactions)
                      .HasForeignKey(t => t.ToUserId)
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
                entity.HasIndex(e => e.UserId);
            });

            // KYCDocument Configuration
            modelBuilder.Entity<KYCDocument>(entity =>
            {
                entity.HasOne(k => k.User)
                      .WithMany()
                      .HasForeignKey(k => k.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(k => k.ReviewedByAdmin)
                      .WithMany(a => a.ReviewedDocuments)
                      .HasForeignKey(k => k.ReviewedByAdminId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.Status);
            });

            // AdminUser Configuration
            modelBuilder.Entity<AdminUser>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.PasswordHash).IsRequired();
            });
        }
    }
}
