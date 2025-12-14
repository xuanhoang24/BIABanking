using BankingSystemAPI.DataLayer;
using BankingSystemAPI.Models.Users.Admin;
using Microsoft.EntityFrameworkCore;

namespace BankingSystemAPI.Services.Admin
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
            int? userId,
            string description,
            string? metadata = null)
        {
            var request = _http.HttpContext?.Request;

            var auditLog = new AuditLog
            {
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                UserId = userId,
                Description = description,
                IpAddress = request?.HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = request?.Headers["User-Agent"].ToString(),
                Metadata = metadata,
                CreatedAt = DateTime.UtcNow
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }

        public async Task LogSuspiciousActivityAsync(int userId, string description)
        {
            await LogAsync(
                AuditAction.SuspiciousActivity,
                "User",
                userId,
                userId,
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
            string? userIdFilter = null,
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

            if (!string.IsNullOrEmpty(userIdFilter) && int.TryParse(userIdFilter, out var userId))
            {
                query = query.Where(a => a.UserId == userId);
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
