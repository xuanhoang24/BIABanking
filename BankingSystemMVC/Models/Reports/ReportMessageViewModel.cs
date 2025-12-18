namespace BankingSystemMVC.Models.Reports
{
    public enum MessageSenderType
    {
        Customer = 1,
        Admin = 2
    }

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
