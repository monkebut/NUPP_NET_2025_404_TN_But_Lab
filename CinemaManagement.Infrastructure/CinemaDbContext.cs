using Microsoft.EntityFrameworkCore;
using CinemaManagement.Infrastructure.Models;

namespace CinemaManagement.Infrastructure
{
    /// <summary>
    /// DbContext for Cinema Management using PostgreSQL
    /// </summary>
    public class CinemaDbContext : DbContext
    {
        public DbSet<PersonModel> Persons { get; set; }
        public DbSet<CustomerModel> Customers { get; set; }
        public DbSet<EmployeeModel> Employees { get; set; }
        public DbSet<FilmModel> Films { get; set; }
        public DbSet<MovieModel> Movies { get; set; }
        public DbSet<CartoonModel> Cartoons { get; set; }
        public DbSet<TicketModel> Tickets { get; set; }

        public CinemaDbContext(DbContextOptions<CinemaDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure PersonModel
            modelBuilder.Entity<PersonModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Age).IsRequired();
            });

            // Configure CustomerModel (Table-per-Type inheritance)
            modelBuilder.Entity<CustomerModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Age).IsRequired();
                entity.Property(e => e.Email).IsRequired().HasMaxLength(200);

                // One-to-many relationship: Customer -> Tickets
                entity.HasMany(e => e.Tickets)
                      .WithOne(t => t.Customer)
                      .HasForeignKey(t => t.CustomerId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure EmployeeModel (Table-per-Type inheritance)
            modelBuilder.Entity<EmployeeModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Age).IsRequired();
                entity.Property(e => e.Position).IsRequired().HasMaxLength(100);
            });

            // Configure FilmModel
            modelBuilder.Entity<FilmModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Genre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Duration).IsRequired();

                // One-to-many relationship: Film -> Tickets
                entity.HasMany(e => e.Tickets)
                      .WithOne(t => t.Film)
                      .HasForeignKey(t => t.FilmId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure MovieModel (Table-per-Type inheritance)
            modelBuilder.Entity<MovieModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Genre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Duration).IsRequired();
                entity.Property(e => e.Director).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Budget).IsRequired().HasColumnType("decimal(18,2)");
            });

            // Configure CartoonModel (Table-per-Type inheritance)
            modelBuilder.Entity<CartoonModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Genre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Duration).IsRequired();
                entity.Property(e => e.Studio).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Is3D).IsRequired();
            });

            // Configure TicketModel
            modelBuilder.Entity<TicketModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CustomerId).IsRequired();
                entity.Property(e => e.FilmId).IsRequired();
                entity.Property(e => e.Price).IsRequired().HasColumnType("decimal(18,2)");

                // Foreign key relationships are configured in CustomerModel and FilmModel
                entity.HasIndex(e => e.CustomerId);
                entity.HasIndex(e => e.FilmId);
            });
        }
    }
}
