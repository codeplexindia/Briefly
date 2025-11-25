using Microsoft.EntityFrameworkCore;
using Briefly.Model.Entities;

namespace Briefly.Database
{
    public class BrieflyDbContext(DbContextOptions<BrieflyDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
    }
}
