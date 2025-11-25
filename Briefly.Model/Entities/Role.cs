using System.ComponentModel.DataAnnotations;

namespace Briefly.Model.Entities
{
    public class Role
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string RoleName { get; set; }
        public required string Permissions { get; set; } // JSON string for permission storage
        public required string Description { get; set; } // New column for permission descriptions
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        // Navigation property for users
        public virtual ICollection<User>? Users { get; set; }
    }
}
