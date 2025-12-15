using BankingSystemAPI.DataLayer;
using BankingSystemAPI.Models.Users.Admin;
using Microsoft.EntityFrameworkCore;

namespace BankingSystemAPI.Services.Admin.Implements
{
    public class AuditService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _http;

        public AuditService(AppDbContext context, IHttpContextAccessor http)
        {
            _context = context;
            _http = http;
        }

        public async Task LogAsync(
            AuditAction action,
            string entityType,
            int entityId,
            int? customerId,
            string description,
            string? metadata = null)
        {
            var request = _http.HttpContext?.Request;

            var auditLog = new AuditLog
            {
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                CustomerId = customerId,
                Description = description,
                IpAddress = request?.HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = request?.Headers["User-Agent"].ToString(),
                Metadata = metadata,
                CreatedAt = DateTime.UtcNow
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }

        public async Task LogSuspiciousActivityAsync(int customerId, string description)
        {
            await LogAsync(
                AuditAction.SuspiciousActivity,
                "Customer",
                customerId,
                customerId,
                description
            );
        }

        public async Task<List<AuditLog>> GetRecentAsync(int take)
        {
            return await _context.AuditLogs
                .AsNoTracking()
                .OrderByDescending(a => a.CreatedAt)
                .Take(take)
                .ToListAsync();
        }

        public async Task<List<AuditLog>> GetFilteredAsync(
            int page,
            int pageSize,
            int? actionFilter = null,
            string? entityFilter = null,
            string? customerIdFilter = null,
            DateTime? dateFilter = null)
        {
            var query = _context.AuditLogs.AsNoTracking();

            // Apply filters
            if (actionFilter.HasValue)
            {
                query = query.Where(a => (int)a.Action == actionFilter.Value);
            }

            if (!string.IsNullOrEmpty(entityFilter))
            {
                query = query.Where(a => a.EntityType.Contains(entityFilter));
            }

            if (!string.IsNullOrEmpty(customerIdFilter) && int.TryParse(customerIdFilter, out var customerId))
            {
                query = query.Where(a => a.CustomerId == customerId);
            }

            if (dateFilter.HasValue)
            {
                var filterDate = dateFilter.Value.Date;
                query = query.Where(a => a.CreatedAt.Date == filterDate);
            }

            return await query
                .OrderByDescending(a => a.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
