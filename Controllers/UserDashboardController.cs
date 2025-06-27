using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OmnitakSupportHub.Controllers
{
    [Authorize(Roles = "End User")]
    public class UserDashboardController : Controller
    {
        public IActionResult Index()
        {
            // End User dashboard, focused on viewing own tickets and creating new ones
            return View();
        }
    }
}