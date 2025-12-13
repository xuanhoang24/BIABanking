using BankingSystemAPI.DataLayer;
using BankingSystemAPI.Models;

namespace BankingSystemAPI.Services
{
    public class AuditService
    {
        private readonly AppDbContext _context;

        public AuditService(AppDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(AuditAction action, string entityType, int entityId,
            int? userId, string description, string? ipAddress = null)
        {
            var auditLog = new AuditLog
            {
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                UserId = userId,
                Description = description,
                IpAddress = ipAddress,
                CreatedAt = DateTime.UtcNow
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }

        public async Task LogSuspiciousActivityAsync(int userId, string description)
        {
            await LogAsync(AuditAction.SuspiciousActivity, "User", userId, userId, description);
        }
    }
}
