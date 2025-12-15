using System.ComponentModel.DataAnnotations;

namespace BankingSystemAPI.Models.DTOs.Accounts
{
    public class DepositRequestDto
    {
        [Required]
        [Range(1, long.MaxValue)]
        public long AmountInCents { get; set; }

        public string? Description { get; set; }
    }
}
