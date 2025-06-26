using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OmnitakSupportHub.Services;

namespace OmnitakSupportHub.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminDashboardController : Controller
    {
        private readonly IAuthService _authService;

        public AdminDashboardController(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<IActionResult> Index()
        {
            // Get pending users for approval
            var pendingUsers = await _authService.GetPendingUsersAsync();
            var availableRoles = await _authService.GetAvailableRolesAsync();

            ViewBag.PendingUsers = pendingUsers;
            ViewBag.AvailableRoles = availableRoles;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ApproveUser(int userId, int roleId)
        {
            var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

            var result = await _authService.ApproveUserAsync(userId, roleId, currentUserId);

            if (result)
            {
                TempData["SuccessMessage"] = "User approved successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to approve user.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RejectUser(int userId)
        {
            var result = await _authService.RejectUserAsync(userId);

            if (result)
            {
                TempData["SuccessMessage"] = "User rejected successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to reject user.";
            }

            return RedirectToAction("Index");
        }
    }
}