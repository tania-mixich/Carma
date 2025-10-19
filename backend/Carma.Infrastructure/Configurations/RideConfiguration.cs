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
        
        builder.Property(r => r.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(r => r.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(r => r.AvailableSeats).IsRequired();
        builder.Property(r => r.Status).HasDefaultValue(Status.Available);
        builder.Property(r => r.Price).IsRequired();
        builder.Property(r => r.PickupTime).IsRequired();

        builder.OwnsOne(r => r.PickupLocation, location =>
        {
            location.Property(l => l.Latitude)
                .HasColumnName("PickupLatitude")
                .IsRequired();

            location.Property(l => l.Longitude)
                .HasColumnName("PickupLongitude")
                .IsRequired();
            
            location.Property(l => l.Address)
                .HasColumnName("PickupAddress")
                .IsRequired()
                .HasMaxLength(255);
        });

        builder.OwnsOne(r => r.DropOffLocation, location =>
        {
            location.Property(l => l.Latitude)
                .HasColumnName("DropOffLatitude")
                .IsRequired();
            
            location.Property(l => l.Longitude)
                .HasColumnName("DropOffLongitude")
                .IsRequired();
            
            location.Property(l => l.Address)
                .HasColumnName("DropOffAddress")
                .IsRequired()
                .HasMaxLength(255);
        });
        
        builder.HasOne(r => r.Organizer)
            .WithMany()
            .HasForeignKey(r => r.OrganizerId);
    }
}