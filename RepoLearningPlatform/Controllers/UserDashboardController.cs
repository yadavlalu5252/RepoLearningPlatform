using Microsoft.AspNetCore.Mvc;
using RepoLearningPlatform.Interfaces;

namespace RepoLearningPlatform.Controllers
{
    public class UserDashboardController : Controller
    {
        private readonly IUserDashboardService uds;
        public UserDashboardController(IUserDashboardService uds)
        {
            this.uds = uds;
        }
        public IActionResult Index()
        {
            int userId = HttpContext.Session.GetInt32("UserId")??0;

            var user = uds.GetUserById(userId);
            return View(user);
        }
    }
}
