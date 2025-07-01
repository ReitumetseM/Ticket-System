using System.ComponentModel.DataAnnotations;

namespace OmnitakSupportHub.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }

        [Required]
        [StringLength(50)]
        public string CategoryName { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Description { get; set; }

        // Navigation Properties
        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
        public virtual ICollection<KnowledgeBase> KnowledgeBaseArticles { get; set; } = new List<KnowledgeBase>();
    }
}