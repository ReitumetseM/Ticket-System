using System;
using System.Collections.Generic;
using System.Linq;
using OmnitakSupportHub.Models;

namespace OmnitakSupportHub.Services
{
    public class InMemoryUserService : IUserServices
    {
        private static readonly List<User> _users = new()
        {
            new User { Username = "alice", Password = "P@ssword1", Role = "Admin" },
            new User { Username = "bob",   Password = "P@ssword2", Role = "User"  }
        };

        public User? ValidateUser(string username, string password)
        {
            return _users.SingleOrDefault(u =>
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)
                && u.Password == password);
        }
    }
}
