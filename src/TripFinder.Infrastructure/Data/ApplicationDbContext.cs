using Microsoft.EntityFrameworkCore;
using RideApp.Domain;
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
            .OnDelete(DeleteBehavior.Restrict); // prevent cascade path

        modelBuilder.Entity<Trip>()
            .HasOne(t => t.Car)
            .WithMany(c => c.Trips)
            .HasForeignKey(t => t.CarId)
            .OnDelete(DeleteBehavior.Cascade); // allow cascade only here

        modelBuilder.Entity<Car>()
            .HasOne(c => c.Driver)
            .WithMany(d => d.Cars)
            .HasForeignKey(c => c.DriverId)
            .OnDelete(DeleteBehavior.Cascade); // keep this cascade
    }

}
