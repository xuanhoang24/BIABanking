using System.ComponentModel.DataAnnotations;
using BankingSystemMVC.Models.Validation;

namespace BankingSystemMVC.Models.ViewModels.Auth
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[!@#$%^&*(),.?""':{}|<>]).{6,}$", 
            ErrorMessage = "Password must contain at least 1 capital letter and 1 special character")]
        public string Password { get; set; } = string.Empty;

        [Required, Compare(nameof(Password))]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Date of Birth")]
        [MinimumAge(16, ErrorMessage = "You must be at least 16 years old to register")]
        public DateTime DateOfBirth { get; set; }

        public string Address { get; set; } = string.Empty;
    }
}
