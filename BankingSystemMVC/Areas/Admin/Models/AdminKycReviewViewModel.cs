using BankingSystemMVC.Models.Kyc;

namespace BankingSystemMVC.Areas.Admin.Models
{
    public class AdminKycReviewViewModel
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DocumentType DocumentType { get; set; }
        public KYCStatus Status { get; set; }
        public string OriginalFileName { get; set; } = string.Empty;
        public string? ReviewNotes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
