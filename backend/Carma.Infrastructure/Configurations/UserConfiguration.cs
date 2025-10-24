using Carma.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Carma.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.Karma).HasDefaultValue(0);
        builder.Property(u => u.CreatedAt).IsRequired();
        builder.Property(u => u.UpdatedAt).IsRequired();
        
        builder.OwnsOne(u => u.Location, location =>
        {
            location.Property(l => l.Address)
                .HasColumnName("Address")
                .HasMaxLength(255);

            location.Property(l => l.Coordinate)
                .HasColumnName("Coordinate")
                .IsRequired()
                .HasColumnType("geography(Point, 4326)");
            
            location.HasIndex(l => l.Coordinate)
                .HasMethod("gist");
        });
        builder.Navigation(u => u.Location).IsRequired(false);
    }
}