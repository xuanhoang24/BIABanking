using BankingSystemAPI.Application.Dtos.Reports;
using BankingSystemAPI.Application.Services.Implementations.Admin;
using BankingSystemAPI.Application.Services.Interfaces.Report;
using BankingSystemAPI.Domain.Entities.Reports;
using BankingSystemAPI.Domain.Entities.Users.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankingSystemAPI.Controllers.Customers
{
    [ApiController]
    [Route("api/customer/reports")]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly AuditService _auditService;

        public ReportsController(IReportService reportService, AuditService auditService)
        {
            _reportService = reportService;
            _auditService = auditService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateReport([FromBody] CreateReportDto request)
        {
            var customerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(customerIdClaim) || !int.TryParse(customerIdClaim, out int customerId))
                return Unauthorized();

            var report = await _reportService.CreateReportAsync(
                customerId,
                request.Name,
                request.Title,
                request.Description
            );

            await _auditService.LogAsync(
                AuditAction.ReportCreated,
                "Report",
                report.Id,
                customerId,
                $"Customer created report: {request.Title}"
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

        [HttpGet]
        public async Task<IActionResult> GetMyReports()
        {
            var customerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(customerIdClaim) || !int.TryParse(customerIdClaim, out int customerId))
                return Unauthorized();

            var reports = await _reportService.GetCustomerReportsAsync(customerId);

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
            var customerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(customerIdClaim) || !int.TryParse(customerIdClaim, out int customerId))
                return Unauthorized();

            var report = await _reportService.GetReportByIdAsync(id);

            if (report == null)
                return NotFound();

            if (report.CustomerId != customerId)
                return Forbid();

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

        [HttpPost("{id}/messages")]
        public async Task<IActionResult> AddMessage(int id, [FromBody] CreateReportMessageDto request)
        {
            var customerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(customerIdClaim) || !int.TryParse(customerIdClaim, out int customerId))
                return Unauthorized();

            var report = await _reportService.GetReportByIdAsync(id);
            if (report == null)
                return NotFound();

            if (report.CustomerId != customerId)
                return Forbid();

            if (report.Status == ReportStatus.Resolved || report.Status == ReportStatus.Closed)
                return BadRequest(new { error = "Cannot send messages on a resolved or closed report" });

            var message = await _reportService.AddMessageAsync(
                id,
                request.Message,
                MessageSenderType.Customer,
                customerId,
                null
            );

            var messageDto = new ReportMessageDto
            {
                Id = message.Id,
                ReportId = message.ReportId,
                Message = message.Message,
                SenderType = message.SenderType,
                SenderName = message.Customer?.FirstName ?? "Customer",
                CreatedAt = message.CreatedAt
            };

            return Ok(messageDto);
        }

        [HttpGet("{id}/messages")]
        public async Task<IActionResult> GetMessages(int id)
        {
            var customerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(customerIdClaim) || !int.TryParse(customerIdClaim, out int customerId))
                return Unauthorized();

            var report = await _reportService.GetReportByIdAsync(id);
            if (report == null)
                return NotFound();

            if (report.CustomerId != customerId)
                return Forbid();

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
