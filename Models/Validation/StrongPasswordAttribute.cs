// Models/Validation/StrongPasswordAttribute.cs
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace OmnitakSupportHub.Models.Validation
{
    public class StrongPasswordAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult("Password is required.");
            }

            var password = value.ToString();

            // Minimum length
            if (password.Length < 8)
            {
                return new ValidationResult("Password must be at least 8 characters.");
            }

            // Uppercase letters
            if (!Regex.IsMatch(password, "[A-Z]"))
            {
                return new ValidationResult("Password must contain at least one uppercase letter (A-Z).");
            }

            // Lowercase letters
            if (!Regex.IsMatch(password, "[a-z]"))
            {
                return new ValidationResult("Password must contain at least one lowercase letter (a-z).");
            }

            // Numbers
            if (!Regex.IsMatch(password, "[0-9]"))
            {
                return new ValidationResult("Password must contain at least one number (0-9).");
            }

            // Special characters
            if (!Regex.IsMatch(password, "[^a-zA-Z0-9]"))
            {
                return new ValidationResult("Password must contain at least one special character.");
            }

            // Common weak passwords
            var weakPasswords = new[] {
                "password", "12345678", "qwertyui", "11111111", "00000000",
                "abcdefgh", "admin123", "letmein", "welcome", "monkey"
            };

            if (weakPasswords.Contains(password.ToLower()))
            {
                return new ValidationResult("Password is too common. Please choose a stronger password.");
            }

            return ValidationResult.Success;
        }
    }
}