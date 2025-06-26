using System;
using OmnitakSupportHub.Models;
using OmnitakSupportHub.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OmnitakSupportHub.Service
{
    public class InMemoryUserService : IUserServices // changed from IUserService to IUserServices
    {
        private static readonly List<User> _users = new()
        {
            new() { Username = "alice", Password = "P@ssword1", Role = "Admin" },
            new() { Username = "bob",   Password = "P@ssword2", Role = "User"  }
        };

        public User? ValidateUser(string username, string password) // renamed from Validate
        {
            return _users.SingleOrDefault(u =>
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)
                && u.Password == password);
        }
    }
}