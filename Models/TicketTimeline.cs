using System.ComponentModel.DataAnnotations;

namespace OmnitakSupportHub.Models
{
    public class TicketTimeline
    {
        [Key]
        public int TimelineID { get; set; }

        public DateTime? ExpectedResolution { get; set; }
        public DateTime? ActualResolution { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        // Foreign Key
        public int TicketID { get; set; }

        // Navigation Property
        public virtual Ticket Ticket { get; set; } = null!;
    }
}
