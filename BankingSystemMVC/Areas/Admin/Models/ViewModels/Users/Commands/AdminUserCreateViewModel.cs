using System.ComponentModel.DataAnnotations;

namespace BankingSystemMVC.Areas.Admin.Models.ViewModels.Users
{
    public class AdminUserCreateViewModel
    {
        [Required, StringLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Role")]
        public int RoleId { get; set; }

        public List<AdminRoleViewModel> AvailableRoles { get; set; } = new();
    }
}
