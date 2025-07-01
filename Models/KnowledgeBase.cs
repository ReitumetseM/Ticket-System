using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OmnitakSupportHub.Models
{
    public class KnowledgeBase
    {
        [Key]
        public int ArticleID { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string Content { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Category { get; set; }

        public int HelpfulCount { get; set; } = 0;
        public bool IsPublished { get; set; } = false;
        public bool IsFeatured { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Foreign Keys
        public int CreatedBy { get; set; }
        public int? LastUpdatedBy { get; set; }

        // Navigation Properties
        public virtual User CreatedByUser { get; set; } = null!;
        public virtual User? LastUpdatedByUser { get; set; }
    }
}
