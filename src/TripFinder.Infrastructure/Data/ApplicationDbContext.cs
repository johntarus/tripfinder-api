using Microsoft.EntityFrameworkCore;
using TripFinder.Domain.Entities;

namespace TripFinder.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}
    
    public DbSet<Trip> Trips { get; set; }
    public DbSet<Driver> Drivers { get; set; }
    public DbSet<Car> Cars { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}