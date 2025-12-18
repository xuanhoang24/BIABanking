using BankingSystemAPI.Models.DTOs.Reports;
using BankingSystemAPI.Models.Reports;
using BankingSystemAPI.Models.Users.Admin;
using BankingSystemAPI.Services.Admin.Implements;
using BankingSystemAPI.Services.Report.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

            await _auditService.LogAsync(
                AuditAction.ReportViewed,
                "Report",
                report.Id,
                report.CustomerId,
                $"Admin viewed report #{report.Id} from customer {report.Customer?.Email}"
            );

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
    }
}
