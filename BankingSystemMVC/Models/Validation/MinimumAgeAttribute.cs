using System.ComponentModel.DataAnnotations;

namespace BankingSystemMVC.Models.Validation
{
    public class MinimumAgeAttribute : ValidationAttribute
    {
        private readonly int _minimumAge;

        public MinimumAgeAttribute(int minimumAge)
        {
            _minimumAge = minimumAge;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime dateOfBirth)
            {
                // Check if date is too far in the past (before 1900)
                if (dateOfBirth.Year < 1900)
                {
                    return new ValidationResult("Please enter a valid date of birth");
                }

                // Check if date is in the future
                if (dateOfBirth.Date > DateTime.Today)
                {
                    return new ValidationResult("Date of birth cannot be in the future");
                }

                var age = DateTime.Today.Year - dateOfBirth.Year;
                if (dateOfBirth.Date > DateTime.Today.AddYears(-age))
                {
                    age--;
                }

                if (age >= _minimumAge)
                {
                    return ValidationResult.Success;
                }

                return new ValidationResult(ErrorMessage ?? $"You must be at least {_minimumAge} years old");
            }

            return new ValidationResult("Invalid date of birth");
        }
    }
}
