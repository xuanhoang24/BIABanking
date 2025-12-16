namespace BankingSystemMVC.Areas.Admin.Models
{
    public class CustomerListViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool IsKYCVerified { get; set; }
        public string Status { get; set; } = string.Empty;
        public int AccountCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CustomerDetailViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; } = string.Empty;
        public bool IsKYCVerified { get; set; }
        public string Status { get; set; } = string.Empty;
        public int FailedLoginAttempts { get; set; }
        public DateTime? LockedUntil { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<AccountSummaryViewModel> Accounts { get; set; } = new();
        public List<TransactionSummaryViewModel> RecentTransactions { get; set; } = new();
    }

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

    public class TransactionSummaryViewModel
    {
        public int Id { get; set; }
        public string TransactionReference { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? FromAccount { get; set; }
        public string? ToAccount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
