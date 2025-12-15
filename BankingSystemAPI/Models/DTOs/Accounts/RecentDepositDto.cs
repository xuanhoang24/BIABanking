namespace BankingSystemAPI.Models.DTOs.Accounts
{
    public class RecentDepositDto
    {
        public int TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }
}
