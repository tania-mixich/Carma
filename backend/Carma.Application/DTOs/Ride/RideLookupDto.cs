using Carma.Domain.Enums;
using NetTopologySuite.Geometries;

namespace Carma.Application.DTOs.Ride;

public class RideLookupDto
{
    public int Id { get; set; }
    public string OrganizerName { get; set; } = string.Empty;
    public double OrganizerKarma { get; set; }
    public string? OrganizerImageUrl { get; set; }
    public Point PickupPoint { get; set; } = null!;
    public Point DropoffPoint { get; set; } = null!;
    public DateTime PickupTime { get; set; }
    public double Price { get; set; }
    public int Seats { get; set; }
    public Status Status { get; set; }
    public int AcceptedCount { get; set; }
}