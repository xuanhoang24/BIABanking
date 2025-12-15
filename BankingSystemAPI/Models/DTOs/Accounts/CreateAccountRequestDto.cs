using BankingSystemAPI.Models.Accounts;
using System.ComponentModel.DataAnnotations;

namespace BankingSystemAPI.Models.DTOs.Accounts
{
    public class CreateAccountRequestDto
    {
        [Required]
        public string AccountName { get; set; }

        [Required]
        public AccountType AccountType { get; set; }
    }
}
