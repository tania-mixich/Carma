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
        
        builder.Property(u => u.UserName).IsRequired().HasMaxLength(255);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(255);
        builder.Property(u => u.Karma).HasDefaultValue(0);
        builder.Property(u => u.ReviewsCount).HasDefaultValue(0);
        builder.Property(u => u.RidesCount).HasDefaultValue(0);
        builder.Property(u => u.CreatedAt).IsRequired();
        builder.Property(u => u.UpdatedAt).IsRequired();
        
        builder.OwnsOne(u => u.Location, location =>
        {
            location.Property(l => l.Coordinate)
                .HasColumnName("Coordinate")
                .HasColumnType("geography(Point, 4326)");
            
            location.HasIndex(l => l.Coordinate)
                .HasMethod("gist");
            
            location.Property(l => l.Address).HasColumnName("Address").HasMaxLength(255);
            location.Property(l => l.City).HasColumnName("City").HasMaxLength(100);
            location.Property(l => l.Country).HasColumnName("Country").HasMaxLength(100);
        });
        builder.Navigation(u => u.Location).IsRequired(false);
    }
}