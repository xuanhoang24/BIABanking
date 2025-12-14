using BankingSystemMVC.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemMVC.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IUserApiClient _usersApi;

        public ProfileController(IUserApiClient usersApi)
        {
            _usersApi = usersApi;
        }

        public async Task<IActionResult> Index()
        {
            var token = Request.Cookies["access_token"];
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");

            var me = await _usersApi.GetMeAsync(token);
            if (me == null)
                return RedirectToAction("Login", "Auth");

            return View(me);
        }
    }
}
