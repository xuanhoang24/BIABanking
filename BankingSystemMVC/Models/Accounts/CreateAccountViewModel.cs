using System.ComponentModel.DataAnnotations;

namespace BankingSystemMVC.Models.Accounts
{
    public class CreateAccountViewModel
    {
        [Required]
        public int AccountType { get; set; }

        [Required, StringLength(100)]
        public string AccountName { get; set; } = string.Empty;
    }
}
