namespace Carma.Domain.Entities;

public class Notification
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public int? RideId { get; set; }
    
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public bool IsRead { get; set; }
    
    public required User User { get; set; }
    public Ride? Ride { get; set; }
}