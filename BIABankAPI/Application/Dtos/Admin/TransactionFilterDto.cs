namespace BankingSystemAPI.Application.Dtos.Admin
{
    public class TransactionFilterDto
    {
        public string? TransactionType { get; set; }
        public string? Status { get; set; }
        public string? Reference { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Limit { get; set; } = 100;
    }
}
