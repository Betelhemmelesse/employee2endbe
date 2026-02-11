using EmployeeHierarchy.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeHierarchy.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Department> Departments { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }

        public DbSet<Position> Positions { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Hierarchy Configuration
            modelBuilder.Entity<Position>()
                .HasOne(p => p.Parent)
                .WithMany(p => p.Children)
                .HasForeignKey(p => p.ParentId)
                .OnDelete(DeleteBehavior.Restrict); // Cannot delete a parent if it has children
        }
    }
}