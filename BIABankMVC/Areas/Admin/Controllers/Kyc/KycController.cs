using BankingSystemMVC.Areas.Admin.Models.Constants;
using BankingSystemMVC.Areas.Admin.Services.Interfaces.Kyc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemMVC.Areas.Admin.Controllers.Kyc
{
    [Area("Admin")]
    [Authorize(Policy = PermissionCodes.KycRead)]
    public class KycController : Controller
    {
        private readonly IAdminKycApiClient _api;

        public KycController(IAdminKycApiClient api)
        {
            _api = api;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var pending = await _api.GetPendingAsync();
            return View(pending);
        }

        [HttpGet]
        public async Task<IActionResult> GetKycList()
        {
            var pending = await _api.GetPendingAsync();
            return PartialView("_KycList", pending);
        }

        [HttpGet]
        public async Task<IActionResult> Review(int id)
        {
            var kyc = await _api.GetForReviewAsync(id);
            if (kyc == null)
                return NotFound();

            return View(kyc);
        }

        [HttpGet]
        public async Task<IActionResult> ViewFile(int id)
        {
            var file = await _api.GetFileAsync(id);
            if (file == null)
                return NotFound();

            return File(file.Bytes, file.ContentType, file.FileName);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = PermissionCodes.KycReview)]
        public async Task<IActionResult> MarkUnderReview(int id)
        {
            await _api.MarkUnderReviewAsync(id);
            return RedirectToAction(nameof(Review), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = PermissionCodes.KycReview)]
        public async Task<IActionResult> Approve(int id)
        {
            await _api.ApproveAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = PermissionCodes.KycReview)]
        public async Task<IActionResult> Reject(int id, string reviewNotes)
        {
            await _api.RejectAsync(id, reviewNotes);
            return RedirectToAction(nameof(Index));
        }
    }
}
