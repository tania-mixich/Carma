using Carma.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Carma.Infrastructure.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");
        builder.HasKey(n => n.Id);
        
        builder.Property(n => n.Title).IsRequired().HasMaxLength(100);
        builder.Property(n => n.Message).IsRequired().HasMaxLength(255);
        builder.Property(n => n.SentAt).HasDefaultValueSql("NOW()");
        builder.Property(n => n.IsRead).HasDefaultValue(false);
        
        builder.HasOne(n => n.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UserId);
        
        builder.HasOne(n => n.Ride)
            .WithMany()
            .HasForeignKey(n => n.RideId);
    }
}