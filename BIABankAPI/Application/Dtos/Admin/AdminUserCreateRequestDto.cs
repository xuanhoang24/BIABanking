using System.ComponentModel.DataAnnotations;

namespace BankingSystemAPI.Application.Dtos.Admin
{
    public class AdminUserCreateRequestDto
    {
        [Required, StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public int RoleId { get; set; }
    }
}
