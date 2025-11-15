using Carma.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;

namespace Carma.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public override string UserName { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public Location? Location { get; set; }
    public double Karma { get; set; }
    public int ReviewsCount { get; set; }
    public int RidesCount { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<Review> GivenReviews { get; set; } = new List<Review>();
    public ICollection<Review> ReceivedReviews { get; set; } = new List<Review>();
    public ICollection<RideParticipant> RideParticipants { get; set; } = new List<RideParticipant>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}