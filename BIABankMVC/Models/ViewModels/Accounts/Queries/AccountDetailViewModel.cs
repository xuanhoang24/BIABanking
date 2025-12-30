namespace BankingSystemMVC.Models.ViewModels.Accounts
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
        public TransactionFilterViewModel Filter { get; set; } = new();
    }
}
