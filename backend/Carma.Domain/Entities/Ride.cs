using Carma.Domain.Enums;
using Carma.Domain.ValueObjects;

namespace Carma.Domain.Entities;

public class Ride
{
    public int Id { get; set; }
    public Guid OrganizerId { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public required Location PickupLocation { get; set; }
    public required Location DropOffLocation { get; set; }
    public DateTime PickupTime { get; set; }
    public double Price { get; set; }
    public int AvailableSeats { get; set; }
    public Status Status { get; set; }
    
    public required User Organizer { get; set; }
    public ICollection<RideParticipant> Participants { get; set; } = new List<RideParticipant>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}