using BankingSystemMVC.Areas.Admin.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminOnly")]
    public class DashboardController : Controller
    {
        private readonly IAdminAuditApiClient _auditApi;
        public DashboardController(IAdminAuditApiClient auditApi)
        {
            _auditApi = auditApi;
        }

        public async Task<IActionResult> Index()
        {
            var logs = await _auditApi.GetRecentAsync();
            return View(logs);
        }
    }
}