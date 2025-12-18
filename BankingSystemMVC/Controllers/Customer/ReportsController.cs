using BankingSystemMVC.Models.Auth;
using BankingSystemMVC.Models.Reports;
using BankingSystemMVC.Services.Interfaces;
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
            return View(reports);
        }

        [HttpGet]
        public IActionResult Create()
        {
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
                ModelState.AddModelError(string.Empty, "Failed to create report. Please try again.");
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
    }
}
