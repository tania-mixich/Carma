namespace Carma.Application.DTOs.Ride;

public record RideQueryDto(
    double PickupLatitude,
    double PickupLongitude,
    double? DropoffLatitude,
    double? DropoffLongitude,
    int PickupRadius = 1000,
    int DropoffRadius = 1000
);