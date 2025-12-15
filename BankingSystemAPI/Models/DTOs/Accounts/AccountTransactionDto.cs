namespace BankingSystemAPI.Models.DTOs.Accounts
{
    public class AccountTransactionDto
    {
        public string Reference { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal PostBalance { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
