using Carma.Domain.Enums;

namespace Carma.Domain.Entities;

public class RideParticipant
{
    public Guid UserId { get; set; }
    public int RideId { get; set; }
    
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public DateTime? AcceptedAt { get; set; }
    public bool IsAccepted { get; set; }
    public RideRole RideRole { get; set; }
    
    public User? User { get; set; }
    public Ride? Ride { get; set; }
}