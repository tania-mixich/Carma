using Carma.Domain.Enums;

namespace Carma.Domain.Entities;

public class RideParticipant
{
    public Guid UserId { get; set; }
    public int RideId { get; set; }
    
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public DateTime? AcceptedAt { get; set; }
    public DateTime? RejectedAt { get; set; }
    public DateTime? LeftAt { get; set; }
    public ParticipantRole Role { get; set; }
    public ParticipantStatus Status { get; set; } = ParticipantStatus.Pending;

    public User User { get; set; } = null!;
    public Ride Ride { get; set; } = null!;
}