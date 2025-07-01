using System.ComponentModel.DataAnnotations;

namespace OmnitakSupportHub.Models
{
    public class Department
    {
        [Key]
        public int DepartmentID { get; set; }

        [Required]
        [StringLength(50)]
        public string DepartmentName { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Description { get; set; }

        // Navigation Properties
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}