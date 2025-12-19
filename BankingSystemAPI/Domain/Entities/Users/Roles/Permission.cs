using System.ComponentModel.DataAnnotations;

namespace BankingSystemAPI.Domain.Entities.Users.Roles
{
    public class Permission
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Code { get; set; } = string.Empty;

        [MaxLength(255)]
        public string Description { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
