using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BankingSystemAPI.Models.Users;
using BankingSystemAPI.Models.Users.Admin;

namespace BankingSystemAPI.Models
{
    public enum DocumentType
    {
        Passport = 1,
        DriversLicense = 2,
        NationalId = 3,
        UtilityBill = 4,
        BankStatement = 5,
        TaxDocument = 6
    }

    public enum KYCStatus
    {
        Pending = 1,
        UnderReview = 2,
        Approved = 3,
        Rejected = 4,
        RequiresAdditionalInfo = 5
    }

    public class KYCDocument
    {
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; }

        [Required]
        public DocumentType DocumentType { get; set; }

        [Required, StringLength(500)]
        public string FilePath { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string OriginalFileName { get; set; } = string.Empty;

        [Required]
        public KYCStatus Status { get; set; } = KYCStatus.Pending;

        [StringLength(1000)]
        public string? ReviewNotes { get; set; }

        public int? ReviewedByAdminId { get; set; }

        [ForeignKey("ReviewedByAdminId")]
        public AdminUser? ReviewedByAdmin { get; set; }

        public DateTime? ReviewedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
