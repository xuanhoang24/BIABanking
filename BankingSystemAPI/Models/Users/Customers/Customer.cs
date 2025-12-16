using BankingSystemAPI.Models.Accounts;
using System.ComponentModel.DataAnnotations;

namespace BankingSystemAPI.Models.Users.Customers
{
    public enum CustomerStatus
    {
        Active = 1,
        Inactive = 2,
        Suspended = 3,
        Closed = 4
    }

    public class Customer
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public string PasswordSalt { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        [StringLength(500)]
        public string Address { get; set; } = string.Empty;

        public bool IsKYCVerified { get; set; } = false;

        public CustomerStatus Status { get; set; } = CustomerStatus.Active;

        public int FailedLoginAttempts { get; set; } = 0;
        public DateTime? LockedUntil { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public List<Account> Accounts { get; set; } = new();
        public List<Transaction> SentTransactions { get; set; } = new();
        public List<Transaction> ReceivedTransactions { get; set; } = new();
    }
}
