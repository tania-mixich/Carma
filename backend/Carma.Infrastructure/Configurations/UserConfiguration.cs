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
        builder.Property(u => u.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(u => u.UpdatedAt).HasDefaultValueSql("NOW()");
        
        builder.OwnsOne(u => u.Location, location =>
        {
            location.Property(l => l.Latitude)
                .HasColumnName("Latitude");

            location.Property(l => l.Longitude)
                .HasColumnName("Longitude");

            location.Property(l => l.Address)
                .HasColumnName("Address")
                .HasMaxLength(255);
        });
        builder.Navigation(u => u.Location).IsRequired(false);
    }
}