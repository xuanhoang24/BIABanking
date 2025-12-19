using System.ComponentModel.DataAnnotations;

namespace BankingSystemAPI.Domain.Entities.Users.Admin
{
    public class AuditLog
    {
        public int Id { get; set; }

        [Required]
        public AuditAction Action { get; set; }

        [Required, StringLength(100)]
        public string EntityType { get; set; } = string.Empty;

        [Required]
        public int EntityId { get; set; }

        public int? CustomerId { get; set; }

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
