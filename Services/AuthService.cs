
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using OmnitakSupportHub.Models;
using System.Security.Cryptography;
using System.Text;


namespace OmnitakSupportHub.Services
{
    public class AuthService : IAuthService
    {
        private readonly OmnitakContext _context;
        private const string SALT = "OmnitakSalt2024";
        private readonly IEmailSender _emailSender; // Add email service
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(OmnitakContext context, IEmailSender emailSender, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _emailSender = emailSender;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AuthResult> RegisterAsync(RegisterModel model)
        {
            // 1) email uniqueness
            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                return new AuthResult
                {
                    Success = false,
                    Message = "Email address already exists."
                };

            // 2) create user (unapproved by default)
            var user = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                PasswordHash = HashPassword(model.Password),
                HashAlgorithm = "SHA256",
                Department = await _context.Departments.FirstOrDefaultAsync(d => d.DepartmentName == model.Department),
                IsApproved = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // 3) audit
            await CreateAuditLogAsync(
                userId: user.UserID,
                action: "REGISTER",
                targetType: "User",
                targetId: user.UserID,
                details: "User registered (pending approval)"
            );

            return new AuthResult
            {
                Success = true,
                Message = "Registration successful; awaiting admin approval."
            };
        }

        public async Task<AuthResult> LoginAsync(LoginModel model)
        {
            // 1) find user + roles
            var user = await _context.Users
                    .Include(u => u.Role)
                    .SingleOrDefaultAsync(u => u.Email == model.Email);


            // 2) verify credentials
            if (user == null || !VerifyPassword(model.Password, user.PasswordHash))
                return new AuthResult
                {
                    Success = false,
                    Message = "Invalid email or password."
                };

            // 3) check flags
            if (!user.IsActive)
                return new AuthResult { Success = false, Message = "Account is inactive." };

            if (!user.IsApproved)
                return new AuthResult { Success = false, Message = "Account pending approval." };

            // 4) audit
            await CreateAuditLogAsync(
                userId: user.UserID,
                action: "LOGIN",
                details: "User logged in."
            );

            // 5) return user (and token placeholder)
            return new AuthResult
            {
                Success = true,
                Message = "Login successful.",
                User = user
            };
        }

        public async Task<bool> ApproveUserAsync(int userId, int roleId, int approvedById)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.IsApproved = true;
            user.ApprovedAt = DateTime.UtcNow;
            user.ApprovedBy = approvedById;
            user.RoleID = roleId;

            await _context.SaveChangesAsync();
            // Send approval email to user
            await SendApprovalEmail(user);

            await CreateAuditLogAsync(
                userId: approvedById,
                action: "APPROVE_USER",
                targetType: "User",
                targetId: userId,
                details: $"Assigned role ID {roleId}."
            );

            return true;
        }
        private async Task SendApprovalEmail(User user)
        {
            try
            {
                // Generate login URL
                var request = _httpContextAccessor.HttpContext?.Request;
                var baseUrl = $"{request?.Scheme}://{request?.Host}";
                var loginUrl = $"{baseUrl}/Account/Login";

                var subject = "Your Account Has Been Approved!";
                var message = $@"
                <h3>Welcome to Omnitak Support Hub, {user.FullName}!</h3>
                <p>Your account has been approved by an administrator.</p>
                <p>You can now log in to the system using your credentials:</p>
                <p><a href='{loginUrl}'>Login to Omnitak Support Hub</a></p>
                <p>Best regards,<br>Omnitak Support Team</p>";

                await _emailSender.SendEmailAsync(user.Email, subject, message);
            }
            catch (Exception ex)
            {
                // Log error but don't block approval
                await CreateAuditLogAsync(
                    userId: user.UserID,
                    action: "EMAIL_ERROR",
                    details: $"Approval email failed: {ex.Message}"
                );
            }
        }
        public async Task<bool> RejectUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.IsActive = false;
            await _context.SaveChangesAsync();

            await CreateAuditLogAsync(
                userId: userId,
                action: "REJECT_USER",
                targetType: "User",
                targetId: userId,
                details: "User rejected/deactivated."
            );

            return true;
        }

        public async Task<List<User>> GetPendingUsersAsync()
        {
            return await _context.Users
                .Where(u => !u.IsApproved && u.IsActive)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> ToggleUserActiveAsync(int userId, int performedById)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.IsActive = !user.IsActive;
            await _context.SaveChangesAsync();

            await CreateAuditLogAsync(
                userId: performedById,
                action: user.IsActive ? "REACTIVATE_USER" : "DEACTIVATE_USER",
                targetType: "User",
                targetId: userId,
                details: user.IsActive ? "User account reactivated." : "User account deactivated."
            );

            return true;
        }

        public async Task<bool> UpdateUserAsync(int userId, string fullName, string? department, int roleId, int? teamId, int modifiedById)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.FullName = fullName;

            // Look up the Department entity by name, or set to null if not provided
            if (!string.IsNullOrWhiteSpace(department))
            {
                user.Department = await _context.Departments
                    .FirstOrDefaultAsync(d => d.DepartmentName == department);
            }
            else
            {
                user.Department = null;
            }

            user.RoleID = roleId;
            user.TeamID = teamId;

            await _context.SaveChangesAsync();

            await CreateAuditLogAsync(
                userId: modifiedById,
                action: "UPDATE_USER",
                targetType: "User",
                targetId: userId,
                details: $"Updated name/department/role/team"
            );

            return true;
        }


        public async Task<List<Role>> GetAvailableRolesAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<List<User>> GetActiveUsersAsync()
        {
            return await _context.Users
                .Where(u => u.IsActive && u.IsApproved)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> CreateAuditLogAsync(
            int userId,
            string action,
            string? targetType = null,
            int? targetId = null,
            string? details = null,
            string? ipAddress = null)
        {
            var log = new AuditLog
            {
                UserID = userId,
                Action = action,
                TargetType = targetType,
                TargetID = targetId,
                Details = details,
                PerformedAt = DateTime.UtcNow
            };
            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
            return true;
        }

        public string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var data = Encoding.UTF8.GetBytes(SALT + password);
            return Convert.ToBase64String(sha.ComputeHash(data));
        }

        public bool VerifyPassword(string password, string hash)
            => HashPassword(password) == hash;
    }
}
