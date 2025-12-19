using System.ComponentModel.DataAnnotations;

namespace BankingSystemAPI.Application.Dtos.Transactions
{
    public class PaymentRequestDto
    {
        [Required]
        public int AccountId { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(150)]
        public string Merchant { get; set; } = string.Empty;
    }
}
