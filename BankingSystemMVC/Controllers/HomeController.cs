using Microsoft.AspNetCore.Mvc;

namespace BankingSystemMVC.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
