using System.ComponentModel.DataAnnotations;

namespace BankingSystemMVC.Models.ViewModels.Accounts.Transactions
{
    public class DepositViewModel
    {
        [Required]
        public int AccountId { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }
    }
}
