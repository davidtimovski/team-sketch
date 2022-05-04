using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TeamSketch.Common.ApiModels;

namespace TeamSketch.Services;

public static class HttpProxy
{
    private readonly static HttpClient HttpClient = new();
    private readonly static JsonSerializerOptions SerializerSettings = new()
    {
        PropertyNameCaseInsensitive = true
    };

    static HttpProxy()
    {
        HttpClient.BaseAddress = new Uri(Globals.ServerUri + "/api/");
        HttpClient.DefaultRequestHeaders.Add(HttpRequestHeader.Accept.ToString(), "application/json");
    }

    public static async Task<JoinRoomValidationResult> ValidateJoinRoomAsync(string room, string user)
    {
        var response = await HttpClient.GetAsync($"rooms/{room}/validate-join/{user}");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<JoinRoomValidationResult>(content, SerializerSettings);
    }

    public static async Task<List<string>> GetUsersInRoomAsync(string room)
    {
        var response = await HttpClient.GetAsync($"rooms/{room}/users");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<List<string>>(content, SerializerSettings);
    }
}
