using NetTopologySuite.Geometries;

namespace Carma.Domain.ValueObjects;

public class Location
{
    public Point Coordinate { get; private set; }
    
    protected Location()
    {
        
    }
    
    public Location(double latitude, double longitude)
    {
        if (latitude is < -90 or > 90)
        {
            throw new ArgumentOutOfRangeException(nameof(latitude));
        }
        if (longitude is < -180 or > 180)
        {
            throw new ArgumentOutOfRangeException(nameof(longitude));
        }
        
        Coordinate = new Point(longitude, latitude) { SRID = 4326 };
    }
}