using BankingSystemAPI.Models.Users.Customers;
using System.ComponentModel.DataAnnotations;

namespace BankingSystemAPI.Models.Reports
{
    public enum ReportStatus
    {
        Pending = 1,
        UnderReview = 2,
        Resolved = 3,
        Closed = 4
    }

    public class Report
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required, StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        public ReportStatus Status { get; set; } = ReportStatus.Pending;

        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
