using BankingSystemAPI.Domain.Entities.Accounts;
using System.ComponentModel.DataAnnotations;

namespace BankingSystemAPI.Application.Dtos.Accounts
{
    public class CreateAccountRequestDto
    {
        [Required]
        public string? AccountName { get; set; }

        [Required]
        public AccountType AccountType { get; set; }
    }
}
