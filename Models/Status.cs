using System.ComponentModel.DataAnnotations;

namespace OmnitakSupportHub.Models
{
    public class Status
    {
        [Key]
        public int StatusID { get; set; }

        [Required]
        [StringLength(20)]
        public string StatusName { get; set; } = string.Empty;

        // Navigation Properties
        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}