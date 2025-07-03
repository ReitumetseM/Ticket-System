using System.ComponentModel.DataAnnotations;
using System;

namespace OmnitakSupportHub.Models
{
    public class TicketTimeline
    {
        [Key]
        public int TimelineID { get; set; }

        public int TicketID { get; set; }
        public required virtual Ticket Ticket { get; set; }

        public DateTime? ExpectedResolution { get; set; }
        public DateTime? ActualResolution { get; set; }
    }
}
