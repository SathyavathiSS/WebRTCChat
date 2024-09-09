// SharedData/Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using SharedData.Models;

namespace SharedData.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configure the User entity
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();  // Ensure usernames are unique
        }
    }
}
