using System;
using System.Collections.Generic;
using System.Linq;
using OmnitakSupportHub.Models;

namespace OmnitakSupportHub.Services
{
    public class InMemoryUserService : IUserServices
    {
        // Store credentials separately since User model might not have Password
        private static readonly Dictionary<string, string> _passwords = new Dictionary<string, string>
        {
            { "alice", "P@ssword1" },
            { "bob", "P@ssword2" }
        };

        private static readonly List<User> _users = new List<User>
        {
            new User { Username = "alice", Role = "Admin" },
            new User { Username = "bob", Role = "User" }
        };

        public User ValidateUser(string username, string password)
        {
            // Check if password matches
            if (_passwords.ContainsKey(username.ToLower()) &&
                _passwords[username.ToLower()] == password)
            {
                // Return the user if password is correct
                return _users.FirstOrDefault(u =>
                    string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase));
            }

            return null;
        }
    }
}}