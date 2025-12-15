namespace BankingSystemMVC.Models.Accounts.Transactions
{
    public class TransactionConfirmViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;

        public string FromAccountNumber { get; set; } = string.Empty;
        public string ToAccountNumber { get; set; } = string.Empty;

        public decimal Amount { get; set; }
        public string Reference { get; set; } = string.Empty;
    }
}
