using Carma.Domain.ValueObjects;

namespace Carma.Domain.Factories;

public static class LocationFactory
{
    public static Location CreateUnknown(double latitude, double longitude)
    {
        return new Location(latitude, longitude, "Unknown address", null, null);
    }
}