﻿using System.Collections.Concurrent;
using System.Net;
using System.Text.Json;

namespace TeamSketch.Web.Services;

public interface ILiveViewService
{
    Task AddAsync(string connectionId, string? ipAddress);
    void Remove(string connectionId);
    List<Location> GetDistinctLocations();
}

public sealed class LiveViewService : ILiveViewService
{
    private static readonly HttpClient HttpClient = new();
    private static readonly JsonSerializerOptions SerializerSettings = new()
    {
        PropertyNameCaseInsensitive = true
    };
    private readonly ConcurrentDictionary<string, Location> _locations = new();

    public LiveViewService()
    {
        HttpClient.DefaultRequestHeaders.Add(HttpRequestHeader.Accept.ToString(), "application/json");
    }

    public async Task AddAsync(string connectionId, string? ipAddress)
    {
        if (ipAddress is null)
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
        if (data is null)
        {
            return;
        }

        _locations.TryAdd(connectionId, data);
    }

    public void Remove(string connectionId)
    {
        _locations.TryRemove(connectionId, out Location? _);
    }

    public List<Location> GetDistinctLocations()
    {
        return _locations.Values.Distinct().ToList();
    }
}

public record Location(string Country, string City, double Lat, double Lon);
