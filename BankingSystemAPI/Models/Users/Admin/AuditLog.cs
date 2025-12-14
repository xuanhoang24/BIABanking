using System.ComponentModel.DataAnnotations;

namespace BankingSystemAPI.Models.Users.Admin
{
    public enum AuditAction
    {
        UserRegistration = 1,
        UserLogin = 2,
        UserLogout = 3,
        AccountCreated = 4,
        AccountFrozen = 5,
        AccountUnfrozen = 6,
        PasswordChanged = 7,
        ProfileUpdated = 8,
        AdminLogin = 9,
        AdminLogout = 10,
        AdminActionPerformed = 11,
        SuspiciousActivity = 12,
        TransactionInitiated = 13,
        TransactionCompleted = 14,
        TransactionFailed = 15,
        KYCSubmitted = 16,
        KYCApproved = 17,
        KYCRejected = 18,
        KYCDocumentUploaded = 19
    }

    public class AuditLog
    {
        public int Id { get; set; }

        [Required]
        public AuditAction Action { get; set; }

        [Required, StringLength(100)]
        public string EntityType { get; set; } = string.Empty;

        [Required]
        public int EntityId { get; set; }

        public int? UserId { get; set; }

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [StringLength(45)]
        public string? IpAddress { get; set; }

        [StringLength(500)]
        public string? UserAgent { get; set; }

        public string? Metadata { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
