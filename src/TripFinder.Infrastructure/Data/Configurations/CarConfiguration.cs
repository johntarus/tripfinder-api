using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripFinder.Domain.Entities;

namespace TripFinder.Infrastructure.Data.Configurations;

public class CarConfiguration : IEntityTypeConfiguration<Car>
{
    public void Configure(EntityTypeBuilder<Car> builder)
    {
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Make)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(c => c.Model)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(c => c.LicensePlate)
            .IsRequired()
            .HasMaxLength(20);
            
        builder.Property(c => c.Color)
            .HasMaxLength(30);
            
        builder.Property(c => c.Photo)
            .HasMaxLength(500);
    }
}