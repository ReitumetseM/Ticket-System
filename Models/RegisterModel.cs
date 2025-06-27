using System.ComponentModel.DataAnnotations;

namespace OmnitakSupportHub.Models
{
    public class RegisterModel
    {
        [Required]
        [Display(Name = "Full Name")]
        [StringLength(100, ErrorMessage = "Full name must be at most 100 characters.")]
        public string FullName { get; set; } = "";

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6,
            ErrorMessage = "Password must be between 6 and 100 characters.")]
        public string Password { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = "";

        [Required]
        [Display(Name = "Department")]
        [StringLength(50)]
        public string Department { get; set; } = "";
    }
}

