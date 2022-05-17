using System.Collections.Concurrent;
using System.Net;
using System.Text.Json;

namespace TeamSketch.Web.Services;

public interface ILiveLocationsService
{
    Task AddAsync(string connectionId, string? ipAddress);
    void Remove(string connectionId);
    List<Location> GetAll();
}

public class LiveLocationsService : ILiveLocationsService
{
    private readonly static HttpClient HttpClient = new();
    private readonly static JsonSerializerOptions SerializerSettings = new()
    {
        PropertyNameCaseInsensitive = true
    };
    private readonly ConcurrentDictionary<string, Location> _locations = new();

    public LiveLocationsService()
    {
        HttpClient.DefaultRequestHeaders.Add(HttpRequestHeader.Accept.ToString(), "application/json");
    }

    public async Task AddAsync(string connectionId, string? ipAddress)
    {
        if (ipAddress == null)
        {
            return;
        }

        var response = await HttpClient.GetAsync(new Uri($"http://ip-api.com/json/{ipAddress}?fields=country,city,lat,lon"));
        if (!response.IsSuccessStatusCode)
        {
            return;
        }

        var content = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<Location>(content, SerializerSettings);
        if (data == null)
        {
            return;
        }

        _locations.TryAdd(connectionId, data);
    }

    public void Remove(string connectionId)
    {
        _locations.TryRemove(connectionId, out Location? _);
    }

    public List<Location> GetAll()
    {
        return _locations.Values.ToList();
    }
}

public record Location(string Country, string City, double Lat, double Lon);
