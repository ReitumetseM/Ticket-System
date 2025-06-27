using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;

namespace OmnitakSupportHub.Models
{
    public class SupportTeam
    {
        [Key]
        public int TeamID { get; set; }

        [Required]
        [StringLength(100)]
        public string TeamName { get; set; } = null!;

        [StringLength(255)]
        public string? Description { get; set; }

        [StringLength(100)]
        public string? Specialization { get; set; }

        // Foreign Key
        public int? TeamLeadID { get; set; }

        // Navigation Properties
        public virtual User? TeamLead { get; set; }
        public virtual ICollection<User> Users { get; set; } = new List<User>();
        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}