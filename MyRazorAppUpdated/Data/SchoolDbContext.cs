using Microsoft.EntityFrameworkCore;
using RazorApp.Models;

namespace RazorApp.Data
{
    public class SchoolDbContext : DbContext
    {
        public SchoolDbContext(DbContextOptions<SchoolDbContext> options)
            : base(options)
        {
        }

        public DbSet<Class> Classes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configure the Class entity
            modelBuilder.Entity<Class>(entity =>
            {
                // Set table name explicitly (optional)
                entity.ToTable("Classes");
                
                // Configure primary key
                entity.HasKey(e => e.Id);
                
                // Configure properties
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(e => e.PersonCount)
                    .IsRequired();
                
                entity.Property(e => e.Description)
                    .HasMaxLength(500);
                
                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);
            });
            
            // You can add seed data here if needed
            // modelBuilder.Entity<Class>().HasData(...);
        }
    }
}