namespace BankingSystemMVC.Areas.Admin.Models
{
    public class AccountSummaryViewModel
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
