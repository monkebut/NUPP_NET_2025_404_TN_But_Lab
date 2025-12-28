using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CinemaManagement.Common;

namespace CinemaManagement.Infrastructure
{
    /// <summary>
    /// DbContext that inherits from IdentityDbContext for ASP.NET Core Identity
    /// </summary>
    public class CinemaDbContext : IdentityDbContext<User>
    {
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Cartoon> Cartoons { get; set; }

        public CinemaDbContext(DbContextOptions<CinemaDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Movie entity
            modelBuilder.Entity<Movie>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Genre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Duration).IsRequired();
                entity.Property(e => e.Director).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Budget).IsRequired();
            });

            // Configure Cartoon entity
            modelBuilder.Entity<Cartoon>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Genre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Duration).IsRequired();
                entity.Property(e => e.Studio).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Is3D).IsRequired();
            });
        }
    }
}
