using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using OmnitakSupportHub.Models;

namespace OmnitakSupportHub.Services
{
    public class InMemoryUserService : IAuthService
    {
        // Store credentials separately if User model doesn't contain password
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

        public User? ValidateUser(string username, string password)
        {
            var lowerUsername = username.ToLower();

            // Check if username exists and password matches
            if (_passwords.TryGetValue(lowerUsername, out var storedPassword) &&
                storedPassword == password)
            {
                // Return user info without password
                return _users.FirstOrDefault(u =>
                    string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase));
            }


            return null;
        }
    }

    public class OmnitakContext : DbContext 
    {
        public OmnitakContext(DbContextOptions<OmnitakContext> options) : base(options)
        {
        }
        // DbSet for Users
        public DbSet<User> Users { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure User entity
            modelBuilder.Entity<User>().HasKey(u => u.Username);
            modelBuilder.Entity<User>().Property(u => u.Password).IsRequired();
            modelBuilder.Entity<User>().Property(u => u.Role).IsRequired();
        }
    }
}