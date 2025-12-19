using System.ComponentModel.DataAnnotations;

namespace BankingSystemMVC.Models.ViewModels.Auth
{
    public class RegisterViewModel
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(3)]
        public string Password { get; set; } = string.Empty;

        [Required, Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        public string PhoneNumber { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        public string Address { get; set; } = string.Empty;
    }

}
