namespace BankingSystemMVC.Models.ViewModels.Reports
{
    public class ReportMessageViewModel
    {
        public int Id { get; set; }
        public int ReportId { get; set; }
        public string Message { get; set; } = string.Empty;
        public MessageSenderType SenderType { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
