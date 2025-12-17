using BankingSystemAPI.Models.DTOs.Admin;
using BankingSystemAPI.Models.Users.Roles;
using BankingSystemAPI.Services.Admin.Implements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemAPI.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/audit")]
    [Authorize(Policy = PermissionCodes.DashboardView)]
    public class AdminAuditController : ControllerBase
    {
        private readonly AuditService _audit;

        public AdminAuditController(AuditService audit)
        {
            _audit = audit;
        }

        [HttpGet("recent")]
        public async Task<IActionResult> GetRecent([FromQuery] int count = 10)
        {
            var logs = await _audit.GetRecentAsync(count);

            var result = logs.Select(l => new AuditLogDto
            {
                Id = l.Id,
                ActionNumber = (int)l.Action,
                Entity = $"{l.EntityType} #{l.EntityId}",
                CustomerId = l.CustomerId?.ToString() ?? "System",
                Description = l.Description,
                Date = l.CreatedAt.ToString("MM-dd-yyyy HH:mm:ss"),
                IpAddress = l.IpAddress,
                UserAgent = l.UserAgent,
                Metadata = l.Metadata
            });

            return Ok(result);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            [FromQuery] int? actionFilter = null,
            [FromQuery] string? entityFilter = null,
            [FromQuery] string? userIdFilter = null,
            [FromQuery] DateTime? dateFilter = null)
        {
            var logs = await _audit.GetFilteredAsync(page, pageSize, actionFilter, entityFilter, userIdFilter, dateFilter);

            var result = logs.Select(l => new AuditLogDto
            {
                Id = l.Id,
                ActionNumber = (int)l.Action,
                Entity = $"{l.EntityType} #{l.EntityId}",
                CustomerId = l.CustomerId?.ToString() ?? "System",
                Description = l.Description,
                Date = l.CreatedAt.ToString("MM-dd-yyyy HH:mm:ss"),
                IpAddress = l.IpAddress,
                UserAgent = l.UserAgent,
                Metadata = l.Metadata
            });

            return Ok(result);
        }
    }
}
