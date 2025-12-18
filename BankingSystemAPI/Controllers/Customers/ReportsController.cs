using BankingSystemAPI.Models.DTOs.Reports;
using BankingSystemAPI.Models.Users.Admin;
using BankingSystemAPI.Services.Admin.Implements;
using BankingSystemAPI.Services.Report.Interfaces;
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
    }
}
