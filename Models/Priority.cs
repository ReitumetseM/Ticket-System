using System.ComponentModel.DataAnnotations;

namespace OmnitakSupportHub.Models
{
    public class Priority
    {
        [Key]
        public int PriorityID { get; set; }

        [Required]
        [StringLength(20)]
        public string Label { get; set; } = string.Empty;

        public int Weight { get; set; }

        // Navigation Properties
        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}