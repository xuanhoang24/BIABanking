using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankingSystemAPI.Models.Accounts
{
    public enum EntryType
    {
        Debit = 1,
        Credit = 2
    }

    public class LedgerEntry
    {
        public int Id { get; set; }

        [Required]
        public int TransactionId { get; set; }

        [ForeignKey("TransactionId")]
        public Transaction? Transaction { get; set; }

        [Required]
        public int AccountId { get; set; }

        [ForeignKey("AccountId")]
        public Account? Account { get; set; }

        [Required]
        public EntryType EntryType { get; set; }

        [Required]
        public long AmountInCents { get; set; }

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        public long PostTransactionBalanceInCents { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [NotMapped]
        public decimal Amount => AmountInCents / 100.0m;

        [NotMapped]
        public decimal PostTransactionBalance => PostTransactionBalanceInCents / 100.0m;
    }
}
