using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Briefly.Model.Entities
{
    public class AuditLog
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; } // Foreign key to User table
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
        public required string Action { get; set; } // e.g., "LOGIN", "UPDATE_PROFILE"
        public required string Details { get; set; } // Optional: Additional details about the action
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
