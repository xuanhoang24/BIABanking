using System.ComponentModel.DataAnnotations;

namespace BankingSystemMVC.Areas.Admin.Models
{
    public class AdminLoginViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
