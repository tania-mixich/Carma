namespace Carma.Domain.Entities;

public class Review
{
    public int Id { get; set; }
    public Guid ReviewerId { get; set; }
    public Guid ReviewedUserId { get; set; }
    public int RideId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public double Karma { get; set; }
    public string Text { get; set; } = string.Empty;

    public User Reviewer { get; set; } = null!;
    public User ReviewedUser { get; set; } = null!;
    public Ride Ride { get; set; } = null!;
}