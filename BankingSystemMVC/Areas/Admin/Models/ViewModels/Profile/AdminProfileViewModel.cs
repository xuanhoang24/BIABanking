namespace BankingSystemMVC.Areas.Admin.Models.ViewModels.Profile
{
    public class AdminProfileViewModel
    {
        public string FirstName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
    }
}
