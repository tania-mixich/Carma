namespace Carma.Domain.ValueObjects;

public class Location
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Address { get; set; }

    public Location()
    {
        
    }
    
    public Location(double latitude, double longitude, string? address = null)
    {
        if (latitude is < -90 or > 90)
        {
            throw new ArgumentOutOfRangeException(nameof(latitude));
        }
        if (longitude is < -180 or > 180)
        {
            throw new ArgumentOutOfRangeException(nameof(longitude));
        }
        
        Latitude = latitude;
        Longitude = longitude;
        Address = address;
    }
    
    public double DistanceTo(Location other)
    {
        const int r = 6371;
        var dLat = ToRadians(other.Latitude - Latitude);
        var dLon = ToRadians(other.Longitude - Longitude);
        
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(Latitude)) * Math.Cos(ToRadians(other.Latitude)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return r * c;
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180;
}