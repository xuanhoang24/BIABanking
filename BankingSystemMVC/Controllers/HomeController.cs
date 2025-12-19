using BankingSystemMVC.Services.Interfaces.Customers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICustomerApiClient _customerApi;

        public HomeController(ILogger<HomeController> logger, ICustomerApiClient customerApi)
        {
            _logger = logger;
            _customerApi = customerApi;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Home/Index - IsAuthenticated: {IsAuth}, User: {User}", 
                User.Identity?.IsAuthenticated, 
                User.Identity?.Name ?? "Anonymous");
            
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            _logger.LogInformation("Home/Dashboard - User: {User}", User.Identity?.Name);
            
            var kyc = await _customerApi.GetMyKycAsync();
            ViewBag.IsKycVerified = kyc?.Status == Models.ViewModels.Kyc.KYCStatus.Approved;
            ViewBag.HasKycSubmission = kyc != null;
            
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
