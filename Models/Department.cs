

using System.ComponentModel.DataAnnotations;

namespace OmnitakSupportHub.Models
{
    public class Department
    {
        public int DepartmentId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public static implicit operator Department(string name)
        {
            return new Department { Name = name };
        }

        public static implicit operator string(Department department)
        {
            return department?.Name ?? string.Empty;
        }
    }
}
