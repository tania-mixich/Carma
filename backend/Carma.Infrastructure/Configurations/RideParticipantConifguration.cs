using Carma.Domain.Entities;
using Carma.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Carma.Infrastructure.Configurations;

public class RideParticipantConifguration : IEntityTypeConfiguration<RideParticipant>
{
    public void Configure(EntityTypeBuilder<RideParticipant> builder)
    {
        builder.ToTable("RideParticipants");
        builder.HasKey(rp => new { rp.RideId, rp.UserId });
        
        builder.Property(rp => rp.RequestedAt).HasDefaultValueSql("NOW()");
        builder.Property(rp => rp.IsAccepted).HasDefaultValue(false);
        builder.Property(rp => rp.RideRole).HasDefaultValue(RideRole.NotAssigned);
        
        builder.HasOne(rp => rp.Ride)
            .WithMany(r => r.Participants)
            .HasForeignKey(rp => rp.RideId);
        
        builder.HasOne(rp => rp.User)
            .WithMany(u => u.RideParticipants)
            .HasForeignKey(rp => rp.UserId);
    }
}