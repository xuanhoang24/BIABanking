using System.ComponentModel.DataAnnotations;

namespace BankingSystemAPI.Models.Users
{
    public class User
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

        [Required, StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        [StringLength(500)]
        public string Address { get; set; } = string.Empty;

        public bool IsKYCVerified { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public List<Account> Accounts { get; set; } = new();
        public List<Transaction> SentTransactions { get; set; } = new();
        public List<Transaction> ReceivedTransactions { get; set; } = new();
    }
}
