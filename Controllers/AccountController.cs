using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OmnitakSupportHub.Models;
using OmnitakSupportHub.Models.ViewModels;
using OmnitakSupportHub.Services;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace OmnitakSupportHub.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly OmnitakContext _context;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
        IAuthService authService,
        OmnitakContext context,
        IEmailSender emailSender, // Change to interface
        IConfiguration configuration,
        ILogger<AccountController> logger)

        {
            _authService = authService;
            _context = context;
            _emailSender = emailSender;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Convert ViewModel to Model for AuthService
            var loginModel = new LoginModel
            {
                Email = model.Email,
                Password = model.Password
            };

            var result = await _authService.LoginAsync(loginModel);

            if (result.Success && result.User != null)
            {
                // Create claims for the authenticated user
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, result.User.UserID.ToString()),
                    new Claim(ClaimTypes.Name, result.User.FullName),
                    new Claim(ClaimTypes.Email, result.User.Email),
                    new Claim("Department", result.User.Department?.DepartmentName ?? ""),
                    new Claim("RoleID", result.User.RoleID?.ToString() ?? ""),
                    new Claim("TeamID", result.User.TeamID?.ToString() ?? "")
                };

                // Add role-based claims if Role is loaded
                if (result.User.Role != null)
                {
                    claims.Add(new Claim(ClaimTypes.Role, result.User.Role.RoleName));

                    // Add permission claims
                    if (result.User.Role.CanApproveUsers)
                        claims.Add(new Claim("Permission", "CanApproveUsers"));
                    if (result.User.Role.CanManageTickets)
                        claims.Add(new Claim("Permission", "CanManageTickets"));
                    if (result.User.Role.CanViewAllTickets)
                        claims.Add(new Claim("Permission", "CanViewAllTickets"));
                    if (result.User.Role.CanManageKnowledgeBase)
                        claims.Add(new Claim("Permission", "CanManageKnowledgeBase"));
                    if (result.User.Role.CanViewReports)
                        claims.Add(new Claim("Permission", "CanViewReports"));
                    if (result.User.Role.CanManageTeams)
                        claims.Add(new Claim("Permission", "CanManageTeams"));
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(8)
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authProperties);

                // Redirect based on role
                if (result.User.Role?.RoleName == "Administrator" || result.User.Role?.RoleName == "Support Manager")
                {
                    return RedirectToAction("Index", "AdminDashboard");
                }
                else if (result.User.Role?.RoleName == "Support Agent")
                {
                    return RedirectToAction("Index", "Agent Dashboard");
                }
                else
                {
                    return RedirectToAction("Index", "UserDashboard");
                }
            }

            ModelState.AddModelError("", result.Message);
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            var departments = _context.Departments.ToList();

            var model = new RegisterViewModel // Create ViewModel
            {
                AvailableDepartments = departments.Select(d => new SelectListItem
                {
                    Value = d.DepartmentName,
                    Text = d.DepartmentName
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model) // Receive ViewModel
        {
            if (!ModelState.IsValid)
            {
                // Repopulate departments on failure
                model.AvailableDepartments = _context.Departments
                    .Select(d => new SelectListItem
                    {
                        Value = d.DepartmentName,
                        Text = d.DepartmentName
                    }).ToList();
                return View(model);
            }

            // Convert ViewModel to Model for service
            var registerModel = new RegisterModel
            {
                FullName = model.FullName,
                Email = model.Email,
                Password = model.Password,
                ConfirmPassword = model.ConfirmPassword,
                Department = model.Department
            };

            var result = await _authService.RegisterAsync(registerModel);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                // Send user confirmation email
                await SendUserConfirmationEmail(model.Email);
                return RedirectToAction("RegisterConfirmation");
            }

            // Repopulate departments if registration fails
            model.AvailableDepartments = _context.Departments
                .Select(d => new SelectListItem
                {
                    Value = d.DepartmentName,
                    Text = d.DepartmentName
                }).ToList();
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", result.Message);
            }
           
            return View(model);
        }
        private async Task SendUserConfirmationEmail(string email)
        {
            try
            {
                var subject = "Registration Received - Awaiting Approval";
                var message = $@"
                <h3>Thank you for registering with Omnitak Support Hub!</h3>
                <p>Your account is pending approval by an administrator.</p>
                <p>You'll receive another email once your account has been activated.</p>
                <p>Best regards,<br>Omnitak Support Team</p>";

                await _emailSender.SendEmailAsync(email, subject, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send confirmation email to user");
            }
        }
        [HttpGet]
        public IActionResult RegisterConfirmation()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}