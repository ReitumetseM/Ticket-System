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
        public string Title { get; set; } = string.Empty;

        [Required, Column(TypeName = "nvarchar(max)")]
        public string Description { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ClosedAt { get; set; }

        // Foreign Keys - Updated to use lookup tables
        public int CreatedBy { get; set; }
        public int? AssignedTo { get; set; }
        public int? TeamID { get; set; }
        public int? CategoryID { get; set; }
        public int? StatusID { get; set; }
        public int? PriorityID { get; set; }

        // Navigation Properties
        public virtual User CreatedByUser { get; set; } = null!;
        public virtual User? AssignedToUser { get; set; }
        public virtual SupportTeam? Team { get; set; }
        public virtual Category? Category { get; set; }
        public virtual Status? Status { get; set; }
        public virtual Priority? Priority { get; set; }

        // Related entities
        public virtual ICollection<TicketTimeline> TicketTimelines { get; set; }
            = new List<TicketTimeline>();
        public virtual ICollection<ChatMessage> ChatMessages { get; set; }
            = new List<ChatMessage>();
        public virtual ICollection<Feedback> Feedbacks { get; set; }
            = new List<Feedback>();
    }

    // Keep enums for backward compatibility and application logic
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