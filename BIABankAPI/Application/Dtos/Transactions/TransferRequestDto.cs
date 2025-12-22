using System.ComponentModel.DataAnnotations;

namespace BankingSystemAPI.Application.Dtos.Transactions
{
    public class TransferRequestDto
    {
        [Required]
        public int FromAccountId { get; set; }

        [Required, StringLength(20, MinimumLength = 10)]
        public string ToAccountNumber { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }
    }
}
