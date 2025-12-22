using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Carma.Application.Abstractions;
using Carma.Domain.Factories;
using Carma.Domain.ValueObjects;
using Microsoft.Extensions.Configuration;

namespace Carma.Infrastructure.Services;

public class GeocodingService : IGeocodingService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public GeocodingService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["GeocodingApiKey"]!;
    }

    public async Task<Location> GetLocationFromCoordinatesAsync(double latitude, double longitude)
    {
        var url =
            $"https://maps.googleapis.com/maps/api/geocode/json?latlng={latitude},{longitude}&key={_apiKey}&language=ro";

        var response = await _httpClient.GetFromJsonAsync<GoogleResponse>(url);

        if (response?.Status != "OK" || response.Results is null || response.Results.Count == 0)
        {
            return LocationFactory.CreateUnknown(latitude, longitude);
        }

        var bestResult = response.Results[0];
        
        string fullAddress = bestResult.FormattedAddress;

        string? city = bestResult.AddressComponents.FirstOrDefault(c => c.Types.Contains("locality"))?.LongName;

        if (string.IsNullOrEmpty(city))
        {
            city = bestResult.AddressComponents.FirstOrDefault(c => c.Types.Contains("administrative_area_level_2"))?.LongName;
        }

        string? country = bestResult.AddressComponents.FirstOrDefault(c => c.Types.Contains("country"))?.LongName;
        
        return new Location(latitude, longitude, fullAddress, city, country);
    }
}

public class GoogleResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
    [JsonPropertyName("results")]
    public List<GoogleResult>? Results { get; set; } 
}

public class GoogleResult
{
    [JsonPropertyName("formatted_address")]
    public string FormattedAddress { get; set; } = string.Empty;

    [JsonPropertyName("address_components")]
    public List<AddressComponent> AddressComponents { get; set; } = new();

    [JsonPropertyName("types")]
    public List<string> Types { get; set; } = new();
}

public class AddressComponent
{
    [JsonPropertyName("long_name")]
    public string LongName { get; set; } = string.Empty;
    
    [JsonPropertyName("types")]
    public List<string> Types { get; set; } = new();
}