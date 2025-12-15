using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BankingSystemAPI.Models.Users;

namespace BankingSystemAPI.Models
{
    public enum TransactionType
    {
        Transfer = 1,
        Deposit = 2,
        Withdrawal = 3,
        Fee = 4,
        Interest = 5,
        Refund = 6
    }

    public enum TransactionStatus
    {
        Pending = 1,
        Completed = 2,
        Failed = 3,
        Cancelled = 4,
        Processing = 5
    }

    public class Transaction
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string TransactionReference { get; set; } = string.Empty;

        [Required]
        public TransactionType Type { get; set; }

        [Required]
        public TransactionStatus Status { get; set; } = TransactionStatus.Pending;

        [Required]
        public long AmountInCents { get; set; }

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        // Sender Information
        public int? FromCustomerId { get; set; }
        [ForeignKey("FromCustomerId")]
        public Customer? FromCustomer { get; set; }

        public int? FromAccountId { get; set; }
        [ForeignKey("FromAccountId")]
        public Account? FromAccount { get; set; }

        // Receiver Information
        public int? ToCustomerId { get; set; }
        [ForeignKey("ToCustomerId")]
        public Customer? ToCustomer { get; set; }

        public int? ToAccountId { get; set; }
        [ForeignKey("ToAccountId")]
        public Account? ToAccount { get; set; }

        public string? Metadata { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ProcessedAt { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public List<LedgerEntry> LedgerEntries { get; set; } = new();

        [NotMapped]
        public decimal Amount => AmountInCents / 100.0m;
    }
}
