using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OmnitakSupportHub.Models
{
    public class Ticket
    {
        [Key]
        public int TicketID { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; } = string.Empty;       // ← never null

        [Required, Column(TypeName = "nvarchar(max)")]
        public string Description { get; set; } = string.Empty; // ← never null

        [StringLength(20)]
        public string Status { get; set; } = TicketStatus.Open.ToString();

        public int Priority { get; set; } = (int)TicketPriority.Medium;

        [StringLength(50)]
        public string? Category { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ClosedAt { get; set; }

        // Foreign Keys
        public int CreatedBy { get; set; }
        public int? AssignedTo { get; set; }
        public int? TeamID { get; set; }

        // Navigation Properties
        public virtual User CreatedByUser { get; set; } = null!;   // ← EF populates
        public virtual User? AssignedToUser { get; set; }
        public virtual SupportTeam? Team { get; set; }

        // Related entities
        public virtual ICollection<TicketTimeline> TicketTimelines { get; set; }
            = new List<TicketTimeline>();
        public virtual ICollection<ChatMessage> ChatMessages { get; set; }
            = new List<ChatMessage>();
        public virtual ICollection<Feedback> Feedbacks { get; set; }
            = new List<Feedback>();
    }

    public enum TicketStatus
    {
        Open = 1,
        InProgress,
        Pending,
        Resolved,
        Closed
    }

    public enum TicketPriority
    {
        Low = 1,
        Medium,
        High,
        Critical
    }
}
