using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Home/Index - IsAuthenticated: {IsAuth}, User: {User}", 
                User.Identity?.IsAuthenticated, 
                User.Identity?.Name ?? "Anonymous");
            
            return View();
        }

        [Authorize]
        public IActionResult Dashboard()
        {
            _logger.LogInformation("Home/Dashboard - User: {User}", User.Identity?.Name);
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
