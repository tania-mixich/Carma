namespace Carma.Domain.Entities;

public class Message
{
    public int Id { get; set; }
    public int RideId { get; set; }
    public Guid UserId { get; set; }
    
    public string Text { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    
    public Ride? Ride { get; set; }
    public User? User { get; set; }
}