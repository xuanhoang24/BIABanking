namespace BankingSystemAPI.Application.Dtos.Admin
{
    public class TransactionListDto
    {
        public int Id { get; set; }
        public string TransactionReference { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public int? FromCustomerId { get; set; }
        public string? FromCustomerName { get; set; }
        public string? FromAccountNumber { get; set; }
        public int? ToCustomerId { get; set; }
        public string? ToCustomerName { get; set; }
        public string? ToAccountNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ProcessedAt { get; set; }
    }
}
