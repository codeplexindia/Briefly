using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Briefly.Model.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; } // Store hashed passwords
        public bool IsActive { get; set; } = true; // Active status
        public Guid RoleId { get; set; } // Foreign key to Role table
        [ForeignKey("RoleId")]
        public virtual Role? Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        // Navigation property for audit logs
        public virtual ICollection<AuditLog>? AuditLogs { get; set; }
    }
}
