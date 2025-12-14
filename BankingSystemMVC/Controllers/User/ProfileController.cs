using BankingSystemMVC.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemMVC.Controllers.User
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
            var me = await _usersApi.GetMeAsync();
            if (me == null)
                return RedirectToAction("Login", "Auth");

            return View(me);
        }
    }
}
