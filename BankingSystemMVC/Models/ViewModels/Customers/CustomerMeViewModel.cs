using BankingSystemMVC.Models.Kyc;

namespace BankingSystemMVC.Models.ViewModels.Customers
{
    public class CustomerMeViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsKYCVerified { get; set; }
    }
}
