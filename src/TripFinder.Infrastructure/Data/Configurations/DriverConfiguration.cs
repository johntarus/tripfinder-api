using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripFinder.Domain.Entities;

namespace TripFinder.Infrastructure.Data.Configurations;

public class DriverConfiguration : IEntityTypeConfiguration<Driver>
{
    public void Configure(EntityTypeBuilder<Driver> builder)
    {
        builder.HasKey(d => d.Id);
        
        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(d => d.Rating)
            .HasColumnType("decimal(3,2)");
            
        builder.Property(d => d.ProfilePicture)
            .HasMaxLength(500);
    }
}