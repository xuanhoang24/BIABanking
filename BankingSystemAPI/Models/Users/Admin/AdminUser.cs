using System.ComponentModel.DataAnnotations;

namespace BankingSystemAPI.Models.Users.Admin
{
    public enum AdminRole
    {
        SuperAdmin = 1,
        KYCReviewer = 2,
        TransactionMonitor = 3,
        CustomerSupport = 4,
        Auditor = 5
    }

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
        public AdminRole Role { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime? LastLoginAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public List<KYCDocument> ReviewedDocuments { get; set; } = new();
    }
}
