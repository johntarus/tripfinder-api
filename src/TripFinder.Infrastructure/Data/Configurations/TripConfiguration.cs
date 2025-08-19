using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripFinder.Domain.Entities;

namespace TripFinder.Infrastructure.Data.Configurations;

public class TripConfiguration : IEntityTypeConfiguration<Trip>
{
    public void Configure(EntityTypeBuilder<Trip> builder)
    {
        builder.HasKey(t=>t.Id);
        
        builder.Property(t => t.PickupLocation).IsRequired().HasMaxLength(200);
        builder.Property(t => t.DropoffLocation).IsRequired().HasMaxLength(200);
        builder.Property(t => t.Cost).HasColumnType("decimal(18,2)");
        builder.Property(t => t.Distance).HasColumnType("decimal(18,2)");
        builder.Property(t => t.Rating).HasColumnType("decimal(3,2)");
        
        builder.HasOne(t=>t.Driver).WithMany(d=>d.Trips).HasForeignKey(t=>t.DriverId);
        builder.HasOne(t=>t.Car).WithMany(c=>c.Trips).HasForeignKey(t=>t.CarId);
        
        builder.HasIndex(t=>t.PickupLocation);
        builder.HasIndex(t=>t.DropoffLocation);
        builder.HasIndex(t=>t.RequestDate);
        builder.HasIndex(t=>t.Status);
    }
}