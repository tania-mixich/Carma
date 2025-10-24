using Carma.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Carma.Infrastructure.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("Reviews");
        builder.HasKey(r => r.Id);
        
        builder.Property(r => r.Text).IsRequired().HasMaxLength(255);
        builder.Property(r => r.Karma).IsRequired();
        builder.Property(r => r.CreatedAt).IsRequired();
        
        builder.HasOne(r => r.Reviewer)
            .WithMany(u => u.GivenReviews)
            .HasForeignKey(r => r.ReviewerId);
        
        builder.HasOne(r => r.ReviewedUser)
            .WithMany(u => u.ReceivedReviews)
            .HasForeignKey(r => r.ReviewedUserId);
        
        builder.HasOne(r => r.Ride)
            .WithMany()
            .HasForeignKey(r => r.RideId);
    }
}