using NetTopologySuite.Geometries;

namespace Carma.Domain.ValueObjects;

public class Location
{
    public string? Address { get; set; }
    public Point Coordinate { get; set; }
    
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
        
        Address = address;
        Coordinate = new Point(longitude, latitude) { SRID = 4326 };
    }

    public double DistanceTo(Location other) => Coordinate.Distance(other.Coordinate);
}