using System;
using System.ComponentModel.DataAnnotations;

namespace OmnitakSupportHub.Models
{
    public class Feedback
    {
        [Key]
        public int FeedbackID { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(500)]
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsAnonymous { get; set; } = false;
        public bool IsResolved { get; set; } = false;

        // Foreign Keys
        public int TicketID { get; set; }
        public int UserID { get; set; }

        // Navigation Properties: EF will populate these,
        // so we tell the compiler “I promise they’ll be set”
        public virtual Ticket Ticket { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
