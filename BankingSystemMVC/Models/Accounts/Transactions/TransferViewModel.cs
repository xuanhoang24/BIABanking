using System.ComponentModel.DataAnnotations;

namespace BankingSystemMVC.Models.Accounts.Transactions
{
    public class TransferViewModel
    {
        [Required]
        public int FromAccountId { get; set; }

        // INTERNAL TRANSFER
        public int? ToAccountId { get; set; }

        // EXTERNAL TRANSFER
        public string? ToAccountNumber { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        // UI helper
        [Required]
        public string TransferType { get; set; } = "Internal";
    }
}
