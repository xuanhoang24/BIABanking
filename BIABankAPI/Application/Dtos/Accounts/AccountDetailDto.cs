namespace BankingSystemAPI.Application.Dtos.Accounts
{
    // Used for a single account detail page
    public class AccountDetailDto
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public string Status { get; set; } = string.Empty;

        public List<AccountTransactionDto> RecentTransactions { get; set; } = new();

    }
}
