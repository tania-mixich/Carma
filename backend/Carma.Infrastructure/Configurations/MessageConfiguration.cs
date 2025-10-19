using Carma.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Carma.Infrastructure.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("Messages");
        builder.HasKey(m => m.Id);
        
        builder.Property(m => m.Text).IsRequired().HasMaxLength(255);
        builder.Property(m => m.SentAt).HasDefaultValueSql("NOW()");
        
        builder.HasOne(m => m.User)
            .WithMany(u => u.Messages)
            .HasForeignKey(m => m.UserId);
        
        builder.HasOne(m => m.Ride)
            .WithMany(r => r.Messages)
            .HasForeignKey(m => m.RideId);
    }
}