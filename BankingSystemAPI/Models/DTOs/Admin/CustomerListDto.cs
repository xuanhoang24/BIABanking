namespace BankingSystemAPI.Models.DTOs.Admin
{
    public class CustomerListDto
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

    public class CustomerDetailDto
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
        public List<AccountSummaryDto> Accounts { get; set; } = new();
        public List<TransactionSummaryDto> RecentTransactions { get; set; } = new();
    }

    public class AccountSummaryDto
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class TransactionSummaryDto
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
