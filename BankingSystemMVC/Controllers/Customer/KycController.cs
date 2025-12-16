using BankingSystemMVC.Models.Kyc;
using BankingSystemMVC.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemMVC.Controllers.Customer
{
    [Authorize]
    public class KycController : Controller
    {
        private readonly ICustomerApiClient _customerApi;

        public KycController(ICustomerApiClient customerApi)
        {
            _customerApi = customerApi;
        }

        // GET: /Kyc/Upload
        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        // POST: /Kyc/Upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(UploadKycViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var success = await _customerApi.UploadKycAsync(model);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Failed to upload KYC document");
                return View(model);
            }

            return RedirectToAction("Index", "Profile");
        }

        // GET: /Kyc/Submissions
        [HttpGet]
        public async Task<IActionResult> Submissions()
        {
            var submissions = await _customerApi.GetMyKycSubmissionsAsync();
            return View(submissions);
        }
    }
}
