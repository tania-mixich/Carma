using Carma.Domain.Enums;

namespace Carma.Domain.Entities;

public class RideParticipant
{
    public Guid UserId { get; set; }
    public int RideId { get; set; }
    
    public DateTime RequestedAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public bool IsAccepted { get; set; }
    public RideRole RideRole { get; set; }
    
    public required User User { get; set; }
    public required Ride Ride { get; set; }
}