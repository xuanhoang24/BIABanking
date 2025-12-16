namespace BankingSystemMVC.Models.Kyc
{
    public class KycSubmissionViewModel
    {
        public int Id { get; set; }
        public DocumentType DocumentType { get; set; }
        public KYCStatus Status { get; set; }
        public string? ReviewNotes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}