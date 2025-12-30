using BankingSystemMVC.Areas.Admin.Models.Constants;
using BankingSystemMVC.Areas.Admin.Models.ViewModels.Audit;
using BankingSystemMVC.Areas.Admin.Services.Interfaces.Audit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemMVC.Areas.Admin.Controllers.Audit
{
    [Area("Admin")]
    [Authorize(Policy = PermissionCodes.DashboardView)]
    public class AuditController : Controller
    {
        private readonly IAdminAuditApiClient _auditApi;

        public AuditController(IAdminAuditApiClient auditApi)
        {
            _auditApi = auditApi;
        }

        public async Task<IActionResult> Index(
            int page = 1,
            int pageSize = 50,
            int? actionFilter = null,
            string? entityFilter = null,
            string? customerIdFilter = null,
            string? searchRef = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            var logs = await _auditApi.GetAllAsync(page, pageSize, actionFilter, entityFilter, customerIdFilter, searchRef, fromDate, toDate);
            
            ViewBag.Filter = new AuditFilterViewModel
            {
                Page = page,
                PageSize = pageSize,
                ActionFilter = actionFilter,
                EntityFilter = entityFilter,
                CustomerIdFilter = customerIdFilter,
                SearchRef = searchRef,
                FromDate = fromDate,
                ToDate = toDate
            };
            
            return View(logs);
        }

        [HttpGet]
        public async Task<IActionResult> FilterPartial(
            int page = 1,
            int pageSize = 50,
            int? actionFilter = null,
            string? entityFilter = null,
            string? customerIdFilter = null,
            string? searchRef = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            var logs = await _auditApi.GetAllAsync(page, pageSize, actionFilter, entityFilter, customerIdFilter, searchRef, fromDate, toDate);
            return PartialView("_AuditLogsTable", logs);
        }

        [HttpGet]
        public async Task<IActionResult> Export(
            int? actionFilter = null,
            string? entityFilter = null,
            string? customerIdFilter = null,
            string? searchRef = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            var logs = await _auditApi.GetAllAsync(1, 10000, actionFilter, entityFilter, customerIdFilter, searchRef, fromDate, toDate);
            
            var csv = "Id,Action,Entity,CustomerId,Description,Date,IpAddress,UserAgent\n";
            foreach (var log in logs)
            {
                csv += $"{log.Id},{log.ActionNumber},\"{log.Entity}\",{log.CustomerId},\"{log.Description?.Replace("\"", "\"\"")}\",{log.Date},\"{log.IpAddress}\",\"{log.UserAgent?.Replace("\"", "\"\"")}\"\n";
            }
            
            var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
            return File(bytes, "text/csv", $"audit-logs-{DateTime.Now:yyyyMMdd-HHmmss}.csv");
        }
    }
}