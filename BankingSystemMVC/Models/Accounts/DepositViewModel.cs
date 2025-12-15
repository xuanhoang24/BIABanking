using System.ComponentModel.DataAnnotations;

namespace BankingSystemMVC.Models.Accounts
{
    public class DepositViewModel
    {
        public int AccountId { get; set; }

        [Required]
        [Range(1, long.MaxValue)]
        public decimal Amount { get; set; }

        public string? Description { get; set; }
    }
}
