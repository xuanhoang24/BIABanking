namespace BankingSystemMVC.Models.Accounts
{
    public class AccountDetailViewModel
    {
        public int Id { get; set; }
        public string AccountName { get; set; } = "";
        public string AccountNumber { get; set; } = "";
        public string AccountType { get; set; } = "";
        public string Status { get; set; } = "";
        public decimal Balance { get; set; }

        public List<AccountTransactionViewModel> RecentTransactions { get; set; } = new();
    }

    public class AccountTransactionViewModel
    {
        public DateTime Date { get; set; }
        public string LocalTime { get; set; } = string.Empty;
        public string Type { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal Amount { get; set; }
        public decimal PostBalance { get; set; }
        public string Status { get; set; } = "";
        public string Reference { get; set; } = "";
    }
}
