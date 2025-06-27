using System.ComponentModel.DataAnnotations;

namespace OmnitakSupportHub.Models
{
    public class PasswordReset
    {
        [Key]
        public Guid Token { get; set; } = Guid.NewGuid();

        public DateTime ExpiresAt { get; set; } = DateTime.Now.AddHours(24);
        public bool IsUsed { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UsedAt { get; set; }

        [StringLength(50)]
        public string? IPAddress { get; set; }

        // Foreign Key
        public int UserID { get; set; }

        // Navigation Property
        public virtual User User { get; set; } = null!;

        // Helper properties
        public bool IsExpired => DateTime.Now > ExpiresAt;
        public bool IsValid => !IsUsed && !IsExpired;
    }
}
