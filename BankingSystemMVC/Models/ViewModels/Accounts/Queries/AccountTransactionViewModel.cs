namespace BankingSystemMVC.Models.ViewModels.Accounts
{
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
