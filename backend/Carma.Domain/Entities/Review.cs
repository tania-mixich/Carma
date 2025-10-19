namespace Carma.Domain.Entities;

public class Review
{
    public int Id { get; set; }
    public Guid ReviewerId { get; set; }
    public Guid ReviewedUserId { get; set; }
    public int RideId { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public int Karma { get; set; }
    public string Text { get; set; } = string.Empty;
    
    public required User Reviewer { get; set; }
    public required User ReviewedUser { get; set; }
    public required Ride Ride { get; set; }
}