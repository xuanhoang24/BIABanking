using BankingSystemAPI.Domain.Entities.Users.Customers;

namespace BankingSystemAPI.Application.Dtos.Customers
{
    public class CustomerMeDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsKYCVerified { get; set; }
        public bool HasKycSubmission { get; set; }
        public KYCStatus? CurrentKycStatus { get; set; }
    }
}
