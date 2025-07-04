using OmnitakSupportHub.Models;

namespace OmnitakSupportHub.Models
{
    public class AuthResult
    {
        internal readonly IEnumerable<object> Errors;

        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public string? Token { get; set; }
        public User? User { get; set; }
    }
}
