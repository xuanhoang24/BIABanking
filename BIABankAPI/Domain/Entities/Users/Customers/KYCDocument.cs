using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BankingSystemAPI.Domain.Entities.Users.Admin;

namespace BankingSystemAPI.Domain.Entities.Users.Customers
{
    public class KYCDocument
    {
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; }

        [Required]
        public DocumentType DocumentType { get; set; }

        [Required]
        public byte[] FileData { get; set; } = Array.Empty<byte>();

        [Required, StringLength(100)]
        public string ContentType { get; set; } = string.Empty;

        [Required, StringLength(255)]
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
    }
}
