using OmnitakSupportHub.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace OmnitakSupportHub.Services
{
    public interface IAuthService
    {
        Task<AuthResult> RegisterAsync(RegisterModel model);
        Task<AuthResult> LoginAsync(LoginModel model);
        Task<bool> ApproveUserAsync(int userId, int roleId, int approvedById);
        Task<bool> RejectUserAsync(int userId);
        Task<List<User>> GetPendingUsersAsync();
        Task<List<Role>> GetAvailableRolesAsync();
        Task<bool> CreateAuditLogAsync(
            int userId,
            string action,
            string? targetType = null,
            int? targetId = null,
            string? details = null,
            string? ipAddress = null);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }
}


