using System.ComponentModel.DataAnnotations;
namespace OmnitakSupportHub.Models
{
    public class Role
    {
        [Key]
        public int RoleID { get; set; }

        [Required]
        [StringLength(50)]
        public string RoleName { get; set; } = null!;

        [StringLength(200)]
        public string? Description { get; set; }
    }
}
