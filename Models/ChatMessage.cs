using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OmnitakSupportHub.Models
{
    public class ChatMessage
    {
        [Key]
        public int MessageID { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string Message { get; set; } = string.Empty;   // ← never null

        public DateTime SentAt { get; set; } = DateTime.Now;
        public DateTime? ReadAt { get; set; }

        public bool IsInternal { get; set; } = false;
        public bool IsSystemMessage { get; set; } = false;

        // Foreign Keys
        public int TicketID { get; set; }
        public int UserID { get; set; }

        // Navigation Properties
        public virtual Ticket Ticket { get; set; } = null!;  // ← assigned by EF
        public virtual User User { get; set; } = null!;  // ← assigned by EF
    }
}
