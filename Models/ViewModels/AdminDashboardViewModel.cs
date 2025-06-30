using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using OmnitakSupportHub.Models;

namespace OmnitakSupportHub.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        [Display(Name = "Total Users")]
        public int TotalUsers { get; set; }

        [Display(Name = "Pending Approvals")]
        public List<User> PendingUsers { get; set; } = new();

        [Display(Name = "Active Sessions")]
        public int ActiveSessions { get; set; }

        [Display(Name = "Available Roles")]
        public List<Role> AvailableRoles { get; set; } = new();

        [Display(Name = "Active Users")]
        public List<User> ActiveUsers { get; set; } = new();

    }
}
