using NetTopologySuite.Geometries;

namespace Carma.Domain.ValueObjects;

public class Location
{
    public Point Coordinate { get; private set; }
    public string Address { get; private set; } = string.Empty;
    public string? City { get; private set; } 
    public string? Country { get; private set; }


    protected Location()
    {
        
    }
    
    public Location(double latitude, double longitude, string address, string? city, string? country)
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
        City = city;
        Country = country;
        Coordinate = new Point(longitude, latitude) { SRID = 4326 };
    }
}