using BankingSystemMVC.Models.ViewModels.Kyc;

namespace BankingSystemMVC.Areas.Admin.Models.ViewModels.Kyc
{
    public class AdminKycPendingViewModel
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DocumentType DocumentType { get; set; }
        public KYCStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ReviewerName { get; set; }
    }
}
