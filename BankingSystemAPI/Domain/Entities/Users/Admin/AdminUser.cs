using BankingSystemAPI.Domain.Entities.Users.Customers;
using BankingSystemAPI.Domain.Entities.Users.Roles;
using System.ComponentModel.DataAnnotations;

namespace BankingSystemAPI.Domain.Entities.Users.Admin
{
    public class AdminUser
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public string PasswordSalt { get; set; } = string.Empty;

        [Required]
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        public bool IsActive { get; set; } = true;
        public DateTime? LastLoginAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public List<KYCDocument> ReviewedDocuments { get; set; } = new();
    }
}
