namespace BankingSystemAPI.Models.DTOs.Accounts
{
    // Used for account lists dashboards
    public class AccountSummaryDto
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
