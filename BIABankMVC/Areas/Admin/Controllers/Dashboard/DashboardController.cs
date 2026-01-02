using BankingSystemMVC.Areas.Admin.Models.Constants;
using BankingSystemMVC.Areas.Admin.Models.ViewModels.Audit;
using BankingSystemMVC.Areas.Admin.Models.ViewModels.Dashboard;
using BankingSystemMVC.Areas.Admin.Services.Interfaces.Audit;
using BankingSystemMVC.Areas.Admin.Services.Interfaces.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemMVC.Areas.Admin.Controllers.Dashboard
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
            var systemStatus = await _dashboardApi.GetSystemStatusAsync();

            var viewModel = new DashboardViewModel
            {
                Stats = stats ?? new DashboardStatsViewModel(),
                RecentAuditLogs = logs ?? new List<AuditLogViewModel>(),
                SystemStatus = systemStatus
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetStats()
        {
            var stats = await _dashboardApi.GetDashboardStatsAsync();
            return PartialView("_StatsCards", stats ?? new DashboardStatsViewModel());
        }

        [HttpGet]
        public async Task<IActionResult> GetRecentActivity()
        {
            var logs = await _auditApi.GetRecentAsync();
            return PartialView("_RecentActivity", logs ?? new List<AuditLogViewModel>());
        }

        [HttpGet]
        public async Task<IActionResult> GetSystemStatus()
        {
            var systemStatus = await _dashboardApi.GetSystemStatusAsync();
            return PartialView("_SystemStatus", systemStatus);
        }
    }
}