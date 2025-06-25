using Microsoft.AspNetCore.Mvc;

namespace OmnitakSupportHub.Controllers
{
    public class UserDashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
