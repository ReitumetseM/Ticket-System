using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmnitakSupportHub.Models;
using OmnitakSupportHub.Services;
using System.Security.Claims;

namespace OmnitakSupportHub.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly IAuthService _authService;
        private readonly OmnitakContext _context;

        public AdminController(IAuthService authService, OmnitakContext context)
        {
            _authService = authService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> PendingUsers()
        {
            // Check if user has permission to approve users
            if (!User.HasClaim("CanApproveUsers", "true"))
            {
                return Forbid();
            }

            var pendingUsers = await _authService.GetPendingUsersAsync();
            var roles = await _context.Roles.ToListAsync();

            ViewBag.Roles = roles;
            return View(pendingUsers);
        }

        [HttpPost]
        [Authorize(Policy = "CanApproveUsers")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveUser(int userId, int roleId)
        {
            // ensure caller can approve
            if (!User.HasClaim("CanApproveUsers", "true"))
                return Forbid();

            // extract admin’s userId from claims
            var adminId = int.Parse(User
                .FindFirst(ClaimTypes.NameIdentifier)!
                .Value);

            // now supply approvedById
            var success = await _authService
                .ApproveUserAsync(userId, roleId, adminId);

            TempData[success ? "SuccessMessage" : "ErrorMessage"]
                = success
                    ? "User approved successfully."
                    : "Failed to approve user.";

            return RedirectToAction("PendingUsers");
        }


        [HttpPost]
        public async Task<IActionResult> RejectUser(int userId)
        {
            // Check if user has permission to approve users
            if (!User.HasClaim("CanApproveUsers", "true"))
            {
                return Forbid();
            }

            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.IsActive = false;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "User rejected successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "User not found.";
            }

            return RedirectToAction("PendingUsers");
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            // Check if user has admin permissions
            if (!User.HasClaim("CanApproveUsers", "true"))
            {
                return Forbid();
            }

            var pendingCount = await _context.Users.CountAsync(u => !u.IsApproved && u.IsActive);
            var totalUsers = await _context.Users.CountAsync(u => u.IsActive);
            var totalTickets = await _context.Tickets.CountAsync();
            var openTickets = await _context.Tickets.CountAsync(t => t.Status.StatusName == "Open");


            ViewBag.PendingUsersCount = pendingCount;
            ViewBag.TotalUsers = totalUsers;
            ViewBag.TotalTickets = totalTickets;
            ViewBag.OpenTickets = openTickets;

            return View();
        }
    }
}