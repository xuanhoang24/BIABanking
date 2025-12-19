using BankingSystemAPI.Domain.Entities.Users.Admin;

namespace BankingSystemAPI.Application.Services.Interfaces.Admin
{
    public interface IAuditService
    {
        Task LogAsync(
            AuditAction action,
            string entityType,
            int entityId,
            int? customerId,
            string description,
            string? metadata = null);
        
        Task LogSuspiciousActivityAsync(int customerId, string description);
        
        Task<List<AuditLog>> GetRecentAsync(int take);
        
        Task<List<AuditLog>> GetFilteredAsync(
            int page,
            int pageSize,
            int? actionFilter = null,
            string? entityFilter = null,
            string? customerIdFilter = null,
            DateTime? dateFilter = null);
    }
}
