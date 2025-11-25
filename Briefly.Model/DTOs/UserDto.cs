using System.ComponentModel.DataAnnotations;

namespace Briefly.Model.DTOs
{
    public class UserDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public required string Username { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [MinLength(6)]
        public required string Password { get; set; } // Plain text, will be hashed before storing

        public bool IsActive { get; set; } = true;

        [Required]
        public Guid RoleId { get; set; } // Role assignment
    }
}
