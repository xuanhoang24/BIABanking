using BankingSystemAPI.Domain.Entities.Users.Admin;

namespace BankingSystemAPI.Domain.Entities.Users.Roles
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
