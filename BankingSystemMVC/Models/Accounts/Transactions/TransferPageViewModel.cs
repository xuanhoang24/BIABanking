namespace BankingSystemMVC.Models.Accounts.Transactions
{
    public class TransferPageViewModel
    {
        public TransferViewModel Transfer { get; set; } = new();
        public List<AccountSummaryViewModel> Accounts { get; set; } = new();
    }
}
