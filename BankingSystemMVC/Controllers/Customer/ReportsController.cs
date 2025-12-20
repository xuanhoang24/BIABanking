using BankingSystemMVC.Models.Constants.Auth;
using BankingSystemMVC.Models.ViewModels.Reports;
using BankingSystemMVC.Services.Interfaces.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemMVC.Controllers.Customer
{
    [Authorize(Policy = CustomerPolicies.Authenticated)]
    public class ReportsController : Controller
    {
        private readonly IReportApiClient _reportApi;

        public ReportsController(IReportApiClient reportApi)
        {
            _reportApi = reportApi;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var reports = await _reportApi.GetMyReportsAsync();
            var hasActiveReport = reports.Any(r => r.Status != ReportStatus.Resolved && r.Status != ReportStatus.Closed);
            ViewBag.HasActiveReport = hasActiveReport;
            
            return View(reports);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var reports = await _reportApi.GetMyReportsAsync();
            var hasActiveReport = reports.Any(r => r.Status != ReportStatus.Resolved && r.Status != ReportStatus.Closed);
            
            if (hasActiveReport)
            {
                TempData["ErrorMessage"] = "You already have an active report. Please wait until it is resolved or closed before creating a new one.";
                return RedirectToAction(nameof(Index));
            }
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateReportViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _reportApi.CreateReportAsync(model);

            if (!result.Success)
            {
                if (result.Error?.Contains("active report") == true)
                {
                    TempData["ErrorMessage"] = result.Error;
                    return RedirectToAction(nameof(Index));
                }
                
                ModelState.AddModelError(string.Empty, result.Error ?? "Failed to create report. Please try again.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Report submitted successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var report = await _reportApi.GetReportByIdAsync(id);

            if (report == null)
                return NotFound();

            var messages = await _reportApi.GetMessagesAsync(id);
            ViewBag.Messages = messages;

            return View(report);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(int id, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return RedirectToAction(nameof(Details), new { id });
            }

            var report = await _reportApi.GetReportByIdAsync(id);
            if (report == null)
                return NotFound();

            if (report.Status == ReportStatus.Resolved || report.Status == ReportStatus.Closed)
            {
                return RedirectToAction(nameof(Details), new { id });
            }

            await _reportApi.AddMessageAsync(id, message);

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Close(int id)
        {
            var report = await _reportApi.GetReportByIdAsync(id);
            if (report == null)
                return NotFound();

            if (report.Status == ReportStatus.Closed)
            {
                TempData["ErrorMessage"] = "Report is already closed.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var result = await _reportApi.CloseReportAsync(id);

            if (result.Success)
            {
                TempData["SuccessMessage"] = "Report closed successfully!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to close report. Please try again.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }
    }
}
