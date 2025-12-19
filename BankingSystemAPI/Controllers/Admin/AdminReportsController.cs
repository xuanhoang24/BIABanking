using BankingSystemAPI.Application.Dtos.Reports;
using BankingSystemAPI.Application.Services.Implementations.Admin;
using BankingSystemAPI.Application.Services.Interfaces.Report;
using BankingSystemAPI.Domain.Entities.Reports;
using BankingSystemAPI.Domain.Entities.Users.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankingSystemAPI.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/reports")]
    [Authorize]
    public class AdminReportsController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly AuditService _auditService;

        public AdminReportsController(IReportService reportService, AuditService auditService)
        {
            _reportService = reportService;
            _auditService = auditService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllReports()
        {
            var reports = await _reportService.GetAllReportsAsync();

            var reportDtos = reports.Select(r => new ReportDto
            {
                Id = r.Id,
                Name = r.Name,
                Title = r.Title,
                Description = r.Description,
                Status = r.Status,
                CustomerId = r.CustomerId,
                CustomerEmail = r.Customer?.Email ?? string.Empty,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            }).ToList();

            return Ok(reportDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReportById(int id)
        {
            var report = await _reportService.GetReportByIdAsync(id);

            if (report == null)
                return NotFound();

            var reportDto = new ReportDto
            {
                Id = report.Id,
                Name = report.Name,
                Title = report.Title,
                Description = report.Description,
                Status = report.Status,
                CustomerId = report.CustomerId,
                CustomerEmail = report.Customer?.Email ?? string.Empty,
                CreatedAt = report.CreatedAt,
                UpdatedAt = report.UpdatedAt
            };

            return Ok(reportDto);
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateReportStatus(int id, [FromBody] UpdateReportStatusDto request)
        {
            var report = await _reportService.UpdateReportStatusAsync(id, request.Status);

            if (report == null)
                return NotFound();

            await _auditService.LogAsync(
                AuditAction.ReportStatusUpdated,
                "Report",
                report.Id,
                report.CustomerId,
                $"Admin updated report #{report.Id} status to {request.Status}"
            );

            var reportDto = new ReportDto
            {
                Id = report.Id,
                Name = report.Name,
                Title = report.Title,
                Description = report.Description,
                Status = report.Status,
                CustomerId = report.CustomerId,
                CreatedAt = report.CreatedAt,
                UpdatedAt = report.UpdatedAt
            };

            return Ok(reportDto);
        }

        [HttpPost("{id}/messages")]
        public async Task<IActionResult> AddMessage(int id, [FromBody] CreateReportMessageDto request)
        {
            var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminIdClaim) || !int.TryParse(adminIdClaim, out int adminId))
                return Unauthorized();

            var report = await _reportService.GetReportByIdAsync(id);
            if (report == null)
                return NotFound();

            var message = await _reportService.AddMessageAsync(
                id,
                request.Message,
                MessageSenderType.Admin,
                null,
                adminId
            );

            var messageDto = new ReportMessageDto
            {
                Id = message.Id,
                ReportId = message.ReportId,
                Message = message.Message,
                SenderType = message.SenderType,
                SenderName = "Admin",
                CreatedAt = message.CreatedAt
            };

            return Ok(messageDto);
        }

        [HttpGet("{id}/messages")]
        public async Task<IActionResult> GetMessages(int id)
        {
            var report = await _reportService.GetReportByIdAsync(id);
            if (report == null)
                return NotFound();

            var messages = await _reportService.GetReportMessagesAsync(id);

            var messageDtos = messages.Select(m => new ReportMessageDto
            {
                Id = m.Id,
                ReportId = m.ReportId,
                Message = m.Message,
                SenderType = m.SenderType,
                SenderName = m.SenderType == MessageSenderType.Customer
                    ? (m.Customer?.FirstName ?? "Customer")
                    : "Admin",
                CreatedAt = m.CreatedAt
            }).ToList();

            return Ok(messageDtos);
        }
    }
}
