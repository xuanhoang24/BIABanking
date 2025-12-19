namespace BankingSystemAPI.Application.Dtos.Admin
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
}
