using BankingSystemMVC.Areas.Admin.Models;
using BankingSystemMVC.Areas.Admin.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = PermissionCodes.DashboardView)]
    public class DashboardController : Controller
    {
        private readonly IAdminAuditApiClient _auditApi;
        private readonly IAdminDashboardApiClient _dashboardApi;

        public DashboardController(IAdminAuditApiClient auditApi, IAdminDashboardApiClient dashboardApi)
        {
            _auditApi = auditApi;
            _dashboardApi = dashboardApi;
        }

        public async Task<IActionResult> Index()
        {
            var logs = await _auditApi.GetRecentAsync();
            var stats = await _dashboardApi.GetDashboardStatsAsync();

            var viewModel = new DashboardViewModel
            {
                Stats = stats ?? new DashboardStatsViewModel(),
                RecentAuditLogs = logs ?? new List<AuditLogViewModel>()
            };

            return View(viewModel);
        }
    }
}