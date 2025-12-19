using BankingSystemMVC.Models.Constants.Auth;
using BankingSystemMVC.Models.ViewModels.Kyc;
using BankingSystemMVC.Services.Interfaces.Customers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemMVC.Controllers.Customer
{
    [Authorize(Policy = CustomerPolicies.Authenticated)]
    public class KycController : Controller
    {
        private readonly ICustomerApiClient _customerApi;

        public KycController(ICustomerApiClient customerApi)
        {
            _customerApi = customerApi;
        }

        [HttpGet]
        public async Task<IActionResult> Upload()
        {
            var kyc = await _customerApi.GetMyKycAsync();

            if (kyc != null &&
                (kyc.Status == KYCStatus.Pending || kyc.Status == KYCStatus.UnderReview))
            {
                return RedirectToAction(nameof(ViewMyKyc));
            }

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(UploadKycViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _customerApi.UploadKycAsync(model);

            if (!result.Success)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Your KYC is currently pending review. You cannot upload a new document."
                );
                return View(model);
            }

            return RedirectToAction(nameof(ViewMyKyc));
        }

        [HttpGet]
        public async Task<IActionResult> ViewMyKyc()
        {
            var kyc = await _customerApi.GetMyKycAsync();
            if (kyc == null)
                return RedirectToAction(nameof(Upload));

            return View(kyc);
        }

        [HttpGet]
        public async Task<IActionResult> ViewFile()
        {
            var file = await _customerApi.GetMyKycFileAsync();
            if (file == null)
                return NotFound();

            return File(file.Content, file.ContentType);
        }
    }
}
