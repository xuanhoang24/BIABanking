using BankingSystemAPI.Models.Users.Admin;

namespace BankingSystemAPI.Models.Users.Roles
{
    public class UserRole
    {
        public int Id { get; set; }

        public int AdminUserId { get; set; }
        public AdminUser? AdminUser { get; set; }

        public int RoleId { get; set; }
        public Role? Role { get; set; }
    }
}
