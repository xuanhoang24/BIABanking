using BankingSystemMVC.Areas.Admin.Models;
using BankingSystemMVC.Areas.Admin.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemMVC.Areas.Admin.Controllers
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
            string? userIdFilter = null,
            DateTime? dateFilter = null)
        {
            var logs = await _auditApi.GetAllAsync(page, pageSize, actionFilter, entityFilter, userIdFilter, dateFilter);
            
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.ActionFilter = actionFilter;
            ViewBag.EntityFilter = entityFilter;
            ViewBag.UserIdFilter = userIdFilter;
            ViewBag.DateFilter = dateFilter;
            
            return View(logs);
        }
    }
}