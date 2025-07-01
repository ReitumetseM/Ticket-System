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

        public int HelpfulCount { get; set; } = 0;
        public bool IsPublished { get; set; } = false;
        public bool IsFeatured { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Foreign Keys - Updated to use Category lookup table
        public int CreatedBy { get; set; }
        public int? LastUpdatedBy { get; set; }
        public int? CategoryID { get; set; }

        // Navigation Properties
        public virtual User CreatedByUser { get; set; } = null!;
        public virtual User? LastUpdatedByUser { get; set; }
        public virtual Category? Category { get; set; }
    }
}