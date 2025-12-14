namespace BankingSystemMVC.Models.Users
{
    public class UserMeViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsKYCVerified { get; set; }
    }
}
