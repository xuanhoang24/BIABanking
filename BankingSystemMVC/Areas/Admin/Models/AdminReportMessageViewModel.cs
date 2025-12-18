namespace BankingSystemMVC.Areas.Admin.Models
{
    public enum AdminMessageSenderType
    {
        Customer = 1,
        Admin = 2
    }

    public class AdminReportMessageViewModel
    {
        public int Id { get; set; }
        public int ReportId { get; set; }
        public string Message { get; set; } = string.Empty;
        public AdminMessageSenderType SenderType { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
