namespace BankingSystemAPI.Application.Dtos.Admin
{
    public class TransactionSummaryDto
    {
        public int Id { get; set; }
        public string TransactionReference { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? FromAccount { get; set; }
        public string? ToAccount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
