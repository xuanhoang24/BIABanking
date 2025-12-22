using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BankingSystemMVC.Models.Validation
{
    public class StrongPasswordAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult("Password is required.");
            }

            var password = value.ToString()!;

            if (password.Length < 6)
            {
                return new ValidationResult("Password must be at least 6 characters long.");
            }

            if (!Regex.IsMatch(password, @"[A-Z]"))
            {
                return new ValidationResult("Password must contain at least 1 capital letter.");
            }

            if (!Regex.IsMatch(password, @"[!@#$%^&*(),.?""':{}|<>]"))
            {
                return new ValidationResult("Password must contain at least 1 special character.");
            }

            return ValidationResult.Success;
        }
    }
}
