namespace Carma.Application.DTOs.Location;

public record LocationGetDto(double Latitude, double Longitude, string Address, string City, string Country);