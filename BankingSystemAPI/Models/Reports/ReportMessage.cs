using BankingSystemAPI.Models.Users.Admin;
using BankingSystemAPI.Models.Users.Customers;
using System.ComponentModel.DataAnnotations;

namespace BankingSystemAPI.Models.Reports
{
    public enum MessageSenderType
    {
        Customer = 1,
        Admin = 2
    }

    public class ReportMessage
    {
        public int Id { get; set; }

        public int ReportId { get; set; }
        public Report Report { get; set; } = null!;

        [Required, StringLength(1000)]
        public string Message { get; set; } = string.Empty;

        public MessageSenderType SenderType { get; set; }

        public int? CustomerId { get; set; }
        public Customer? Customer { get; set; }

        public int? AdminUserId { get; set; }
        public AdminUser? AdminUser { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
