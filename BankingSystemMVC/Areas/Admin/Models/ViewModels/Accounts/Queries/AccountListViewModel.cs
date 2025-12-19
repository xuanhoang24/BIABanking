namespace BankingSystemMVC.Areas.Admin.Models
{
    public class AccountListViewModel
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
