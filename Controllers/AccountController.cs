using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using OmnitakSupportHub.Models;
using OmnitakSupportHub.Models.ViewModels;
using OmnitakSupportHub.Services;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

namespace OmnitakSupportHub.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
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
                    new Claim("Department", result.User.Department ?? ""),
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
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var registerModel = new RegisterModel
            {
                Email = model.Email,
                Password = model.Password,
                FullName = model.FullName,
                Department = model.Department
            };

            var result = await _authService.RegisterAsync(registerModel);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", result.Message);
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}