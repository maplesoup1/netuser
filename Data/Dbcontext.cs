
using Microsoft.EntityFrameworkCore;
using Titube.Entities;

namespace Titube.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Media> Media { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.Password).IsRequired();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasMany(e => e.Medias)
                    .WithOne(e => e.User)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            
            modelBuilder.Entity<Media>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FileName).IsRequired();
                entity.Property(e => e.FilePath).IsRequired();
                entity.Property(e => e.FileDescription).IsRequired();
                entity.Property(e => e.FileSize).IsRequired();
                entity.Property(e => e.Type).IsRequired();
            });
        }
    }
}
