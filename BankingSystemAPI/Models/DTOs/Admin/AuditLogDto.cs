namespace BankingSystemAPI.Models.DTOs.Admin
{
    public class AuditLogDto
    {
        public int Id { get; set; }
        public int ActionNumber { get; set; }
        public string Entity { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? Metadata { get; set; }
    }
}
