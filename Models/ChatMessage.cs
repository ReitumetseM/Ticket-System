using System.ComponentModel.DataAnnotations;
using System;

namespace OmnitakSupportHub.Models
{
    public class ChatMessage
    {
        [Key]
        public int MessageID { get; set; }

        public int TicketID { get; set; }
        public required virtual Ticket Ticket { get; set; }

        public int UserID { get; set; }
        public required virtual User User { get; set; }

        [Required]
        public required string Message { get; set; }

        public DateTime SentAt { get; set; }
        public DateTime? ReadAt { get; set; }
    }
}
