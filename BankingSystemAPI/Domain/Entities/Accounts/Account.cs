using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BankingSystemAPI.Domain.Entities.Users.Customers;

namespace BankingSystemAPI.Domain.Entities.Accounts
{
    public class Account
    {
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; }

        [Required, StringLength(20)]
        public string AccountNumber { get; set; } = string.Empty;

        [Required]
        public AccountType AccountType { get; set; }

        [Required]
        public AccountStatus Status { get; set; } = AccountStatus.PendingActivation;

        public long BalanceInCents { get; set; } = 0;

        [StringLength(100)]
        public string AccountName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public List<LedgerEntry> LedgerEntries { get; set; } = new();
        public List<Transaction> DebitTransactions { get; set; } = new();
        public List<Transaction> CreditTransactions { get; set; } = new();

        [NotMapped]
        public decimal Balance => BalanceInCents / 100.0m;
    }
}
