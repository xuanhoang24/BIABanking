using BankingSystemMVC.Models.Kyc;

namespace BankingSystemMVC.Areas.Admin.Models
{
    public class AdminKycPendingViewModel
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DocumentType DocumentType { get; set; }
        public KYCStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
