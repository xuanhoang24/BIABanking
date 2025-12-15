namespace BankingSystemMVC.Models.Accounts
{
    public class RecentDepositViewModel
    {
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }

    public class AccountDetailViewModel
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public string Status { get; set; } = string.Empty;

        public List<RecentDepositViewModel> RecentDeposits { get; set; } = new();

    }
}
