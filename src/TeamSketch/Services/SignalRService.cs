using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using TeamSketch.Utils;

namespace TeamSketch.Services;

public interface ISignalRService
{
    HubConnection Connection { get; }
    Task CreateRoomAsync();
    Task JoinRoomAsync();
    Task JoinRandomRoomAsync();
    Task DrawPointAsync(double x, double y);
    Task DrawLineAsync(List<Point> points);
}

public class SignalRService : ISignalRService
{
    private readonly IAppState _appState;

    public SignalRService(IAppState appState)
    {
        _appState = appState;
        Connection = new HubConnectionBuilder()
           .WithUrl(Globals.ServerUri + "/actionHub")
           .WithAutomaticReconnect()
           .AddMessagePackProtocol()
           .Build();
    }

    public HubConnection Connection { get; }

    public async Task CreateRoomAsync()
    {
        Connection.On<string>("RoomCreated", (room) =>
        {
            _appState.Room = room;
        });

        await Connection.StartAsync();
        await Connection.InvokeAsync("CreateRoom", _appState.Nickname);
    }

    public async Task JoinRoomAsync()
    {
        await Connection.StartAsync();
        await Connection.InvokeAsync("JoinRoom", _appState.Nickname, _appState.Room);
    }

    public async Task JoinRandomRoomAsync()
    {
        await Connection.StartAsync();
        await Connection.InvokeAsync("JoinRandomRoom", _appState.Nickname);
    }

    public async Task DrawPointAsync(double x, double y)
    {
        var data = PayloadConverter.ToBytes(x, y, _appState.BrushSettings.BrushThickness, _appState.BrushSettings.BrushColor);
        await Connection.InvokeAsync("DrawPoint", _appState.Nickname, _appState.Room, data);
    }

    public async Task DrawLineAsync(List<Point> points)
    {
        var data = PayloadConverter.ToBytes(points, _appState.BrushSettings.BrushThickness, _appState.BrushSettings.BrushColor);
        await Connection.InvokeAsync("DrawLine", _appState.Nickname, _appState.Room, data);
    }
}
