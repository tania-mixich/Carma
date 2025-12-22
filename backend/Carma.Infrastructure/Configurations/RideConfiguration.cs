using Carma.Domain.Entities;
using Carma.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Carma.Infrastructure.Configurations;

public class RideConfiguration : IEntityTypeConfiguration<Ride>
{
    public void Configure(EntityTypeBuilder<Ride> builder)
    {
        builder.ToTable("Rides");
        builder.HasKey(r => r.Id);
        
        builder.Property(r => r.CreatedAt).IsRequired();
        builder.Property(r => r.UpdatedAt).IsRequired();
        builder.Property(r => r.Seats).IsRequired();
        builder.Property(r => r.Status).HasDefaultValue(Status.Available)
            .HasConversion<string>()
            .HasMaxLength(50);
        builder.Property(r => r.Price).IsRequired();
        builder.Property(r => r.PickupTime).IsRequired();

        builder.OwnsOne(r => r.PickupLocation, location =>
        { 
            location.Property(l => l.Coordinate)
                .HasColumnName("PickupCoordinate")
                .IsRequired()
                .HasColumnType("geography(Point, 4326)");
            
            location.HasIndex(l => l.Coordinate)
                .HasMethod("gist");
            
            location.Property(l => l.Address)
                .HasColumnName("PickupAddress")
                .HasMaxLength(255)
                .IsRequired();

            location.Property(l => l.City)
                .HasColumnName("PickupCity")
                .HasMaxLength(100);
            
            location.Property(l => l.Country)
                .HasColumnName("PickupCountry")
                .HasMaxLength(100);
        });

        builder.OwnsOne(r => r.DropOffLocation, location =>
        {
            location.Property(l => l.Coordinate)
                .HasColumnName("DropOffCoordinate")
                .IsRequired()
                .HasColumnType("geography(Point, 4326)");
            
            location.HasIndex(l => l.Coordinate)
                .HasMethod("gist");
            
            location.Property(l => l.Address)
                .HasColumnName("DropOffAddress")
                .HasMaxLength(255)
                .IsRequired();
            
            location.Property(l => l.City)
                .HasColumnName("DropOffCity")
                .HasMaxLength(100);
            
            location.Property(l => l.Country)
                .HasColumnName("DropOffCountry")
                .HasMaxLength(100);
        });
        
        builder.HasOne(r => r.Organizer)
            .WithMany()
            .HasForeignKey(r => r.OrganizerId);
    }
}