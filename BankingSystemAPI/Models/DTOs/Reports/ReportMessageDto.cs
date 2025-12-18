using BankingSystemAPI.Models.Reports;

namespace BankingSystemAPI.Models.DTOs.Reports
{
    public class ReportMessageDto
    {
        public int Id { get; set; }
        public int ReportId { get; set; }
        public string Message { get; set; } = string.Empty;
        public MessageSenderType SenderType { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
