using Microsoft.EntityFrameworkCore;
using TripFinder.Domain.Entities;

namespace TripFinder.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Driver> Drivers => Set<Driver>();
    public DbSet<Car> Cars => Set<Car>();
    public DbSet<Trip> Trips => Set<Trip>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Trip>()
            .HasOne(t => t.Driver)
            .WithMany(d => d.Trips)
            .HasForeignKey(t => t.DriverId)
            .OnDelete(DeleteBehavior.Restrict); 

        modelBuilder.Entity<Trip>()
            .HasOne(t => t.Car)
            .WithMany(c => c.Trips)
            .HasForeignKey(t => t.CarId)
            .OnDelete(DeleteBehavior.Cascade); 

        modelBuilder.Entity<Car>()
            .HasOne(c => c.Driver)
            .WithMany(d => d.Cars)
            .HasForeignKey(c => c.DriverId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Time-based queries
        modelBuilder.Entity<Trip>().HasIndex(t => t.RequestDate);

        // Top destinations
        modelBuilder.Entity<Trip>().HasIndex(t => t.DropoffLocation);

        // Search filters
        modelBuilder.Entity<Trip>().HasIndex(t => t.PickupLocation);
        modelBuilder.Entity<Trip>().HasIndex(t => t.Type);
        modelBuilder.Entity<Trip>().HasIndex(t => t.Status);
        modelBuilder.Entity<Trip>().HasIndex(t => t.DistanceKm);
        modelBuilder.Entity<Trip>().HasIndex(t => t.DurationMinutes);

        // Relationship lookups
        modelBuilder.Entity<Trip>().HasIndex(t => t.DriverId);
        modelBuilder.Entity<Trip>().HasIndex(t => t.CarId);
    }

}
