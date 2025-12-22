using BankingSystemMVC.Areas.Admin.Models.Constants;
using BankingSystemMVC.Areas.Admin.Models.ViewModels.Reports;
using BankingSystemMVC.Areas.Admin.Services.Interfaces.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemMVC.Areas.Admin.Controllers.Reports
{
    [Area("Admin")]
    [Authorize(Policy = PermissionCodes.CustomerRead)]
    public class ReportsController : Controller
    {
        private readonly IAdminReportApiClient _reportApi;

        public ReportsController(IAdminReportApiClient reportApi)
        {
            _reportApi = reportApi;
        }

        public async Task<IActionResult> Index()
        {
            var reports = await _reportApi.GetAllReportsAsync();
            return View(reports ?? new List<AdminReportViewModel>());
        }

        public async Task<IActionResult> Details(int id)
        {
            var report = await _reportApi.GetReportByIdAsync(id);
            if (report == null)
            {
                TempData["Error"] = "Report not found";
                return RedirectToAction(nameof(Index));
            }

            var messages = await _reportApi.GetMessagesAsync(id);
            ViewBag.Messages = messages;

            return View(report);
        }

        [HttpPost]
        [Authorize(Policy = PermissionCodes.CustomerManage)]
        public async Task<IActionResult> UpdateStatus(int id, int status)
        {
            var result = await _reportApi.UpdateReportStatusAsync(id, status);
            if (result)
            {
                TempData["Success"] = "Report status updated successfully";
            }
            else
            {
                TempData["Error"] = "Failed to update report status";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(int id, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return RedirectToAction(nameof(Details), new { id });
            }

            await _reportApi.AddMessageAsync(id, message);

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
