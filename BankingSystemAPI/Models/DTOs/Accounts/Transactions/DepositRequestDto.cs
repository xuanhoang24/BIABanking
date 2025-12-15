using System.ComponentModel.DataAnnotations;

namespace BankingSystemAPI.Models.DTOs.Accounts.Transactions
{
    public class DepositRequestDto
    {
        [Required]
        public int AccountId { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }
    }
}
