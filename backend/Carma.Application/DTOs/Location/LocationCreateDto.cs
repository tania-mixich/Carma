namespace Carma.Application.DTOs.Location;

public record LocationCreateDto(double Latitude, double Longitude, string? Address = null, string? City = null, string? Country = null);