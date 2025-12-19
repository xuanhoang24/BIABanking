using BankingSystemAPI.Application.Dtos.Accounts;

namespace BankingSystemAPI.Application.Dtos.Admin
{
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
}
