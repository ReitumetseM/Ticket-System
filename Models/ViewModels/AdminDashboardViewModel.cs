using System.ComponentModel.DataAnnotations;

namespace OmnitakSupportHub.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        [Display(Name = "Total Users")]
        public int TotalUsers { get; set; }

        [Display(Name = "Pending Approvals")]
        public int PendingUsers { get; set; }

        [Display(Name = "Active Sessions")]
        public int ActiveSessions { get; set; }
    }
}
