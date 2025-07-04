using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OmnitakSupportHub.Models;
using OmnitakSupportHub.Services;
using System.Security.Claims;

namespace OmnitakSupportHub.Controllers
{

    [Authorize]
    public class AdminController : Controller
    {
        
        private readonly IEmailSender _emailSender;
        private readonly ILogger<AdminController> _logger;
        private readonly IAuthService _authService;
        private readonly OmnitakContext _context;
        private readonly IConfiguration _configuration;

      

        public AdminController(IAuthService authService,
        OmnitakContext context,
        IEmailSender emailSender,
        ILogger<AdminController> logger, IConfiguration configuration)
        {
            _authService = authService;
            _context = context;
            _emailSender = emailSender;
            _logger = logger;
            _configuration = configuration;
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
            var user = await _context.Users
            .Include(u => u.Department)
            .FirstOrDefaultAsync(u => u.UserID == userId);
            // now supply approvedById
            var success = await _authService
                .ApproveUserAsync(userId, roleId, adminId);

            if (success)
            {
                // Send approval email
                await SendApprovalEmail(user);
                TempData["SuccessMessage"] = "User approved successfully. Notification sent.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to approve user";
            }

            return RedirectToAction("PendingUsers");

        }
        private async Task SendApprovalEmail(User user)
        {
            try
            {
                var subject = "Your OmnitakSupportHub Account Has Been Approved!";
                var message = $@"
            <h3>Welcome to Omnitak Support Hub, {user.FullName}!</h3>
            <p>Your account has been approved by an administrator.</p>
            <p>You can now access the system using your credentials:</p>
            <p><strong>Login URL:</strong> <a href='{Url.Action("Login", "Account", null, "https")}'>
                {Url.Action("Login", "Account", null, "https")}
            </a></p>
            <p>Department: {user.Department?.DepartmentName ?? "N/A"}</p>
            <p>If you have any questions, contact our support team:</p>
            <ul>
                <li>Email: support@omnitak.com</li>
                <li>Phone: +1 (555) 123-4567</li>
            </ul>
            <p>Best regards,<br><strong>Omnitak Support Team</strong></p>";

                await _emailSender.SendEmailAsync(user.Email, subject, message);
            }
            catch (Exception ex)
            {
                // Log error but don't block approval
                _logger.LogError(ex, "Failed to send approval email to {Email}", user.Email);
            }
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