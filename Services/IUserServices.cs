using OmnitakSupportHub.Models;

namespace OmnitakSupportHub.Services
{
    public interface IUserServices
    {
        User? ValidateUser(string username, string password);
    }
}

