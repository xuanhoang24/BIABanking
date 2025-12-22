using System.ComponentModel.DataAnnotations;

namespace BankingSystemAPI.Application.Dtos.Auth
{
    public class RegisterRequestDto
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string PhoneNumber { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        public string Address { get; set; } = string.Empty;
    }
}
