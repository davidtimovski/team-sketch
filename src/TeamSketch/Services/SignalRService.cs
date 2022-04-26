using System;
using System.Threading.Tasks;
using Avalonia.Threading;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using TeamSketch.Utils;

namespace TeamSketch.Services;

public interface ISignalRService
{
    string Nickname { get; }
    string Room { get; }
    Task CreateRoomAsync(string userNickname);
    Task JoinRoomAsync(string userNickname, string joinedRoom);
    Task DisconnectAsync();
    Task DrawPointAsync(double x, double y);
    Task DrawLineAsync(double x1, double y1, double x2, double y2);

    event EventHandler<UserEventArgs> UserJoined;
    event EventHandler<UserEventArgs> UserLeft;
    event EventHandler<DrewEventArgs> UserDrewPoint;
    event EventHandler<DrewEventArgs> UserDrewLine;
    event EventHandler<PongEventArgs> Pong;
    event EventHandler<EventArgs> Disconnected;
    event EventHandler<EventArgs> Reconnected;
}

public class SignalRService : ISignalRService
{
    private const int PingIntervalSeconds = 4;
    private readonly HubConnection _connection;
    private readonly DispatcherTimer _pingTimer = new();
    private DateTime lastPing;

    public SignalRService()
    {
        _connection = new HubConnectionBuilder()
           .WithUrl(Globals.ServerUri + "/actionHub")
           .AddMessagePackProtocol()
           .Build();
    }
    
    public string Nickname { get; private set; }
    public string Room { get; private set; }

    public async Task CreateRoomAsync(string nickname)
    {
        await _connection.StartAsync();

        Nickname = nickname.Trim();

        InitializeHandlersAndPingTimer();

        _connection.On<string>("RoomCreated", (room) =>
        {
            Room = room;
        });

        await _connection.InvokeAsync("CreateRoom", nickname);
    }

    public async Task JoinRoomAsync(string nickname, string room)
    {
        await _connection.StartAsync();

        Nickname = nickname.Trim();
        Room = room;

        InitializeHandlersAndPingTimer();

        await _connection.InvokeAsync("JoinRoom", Nickname, Room);
    }

    public async Task DisconnectAsync()
    {
        _pingTimer.Stop();
        await _connection.StopAsync();
    }

    public async Task DrawPointAsync(double x, double y)
    {
        var data = PayloadConverter.ToBytes(x, y, BrushSettings.BrushThickness, BrushSettings.BrushColor);
        await _connection.InvokeAsync("DrawPoint", Nickname, Room, data);
    }

    public async Task DrawLineAsync(double x1, double y1, double x2, double y2)
    {
        var data = PayloadConverter.ToBytes(x1, y1, x2, y2, BrushSettings.BrushThickness, BrushSettings.BrushColor);
        await _connection.InvokeAsync("DrawLine", Nickname, Room, data);
    }

    public event EventHandler<UserEventArgs> UserJoined;
    public event EventHandler<UserEventArgs> UserLeft;
    public event EventHandler<DrewEventArgs> UserDrewPoint;
    public event EventHandler<DrewEventArgs> UserDrewLine;
    public event EventHandler<PongEventArgs> Pong;
    public event EventHandler<EventArgs> Disconnected;
    public event EventHandler<EventArgs> Reconnected;

    private void InitializeHandlersAndPingTimer()
    {
        _connection.Closed += Connection_Closed;
        _connection.Reconnected += Connection_Reconnected;

        _connection.On<string>("JoinedRoom", (user) =>
        {
            UserJoined.Invoke(this, new UserEventArgs(user));
        });

        _connection.On<string>("LeftRoom", (user) =>
        {
            UserLeft.Invoke(this, new UserEventArgs(user));
        });

        _connection.On<string, byte[]>("DrewPoint", (user, data) =>
        {
            UserDrewPoint.Invoke(this, new DrewEventArgs(user, data));
        });

        _connection.On<string, byte[]>("DrewLine", (user, data) =>
        {
            UserDrewLine.Invoke(this, new DrewEventArgs(user, data));
        });

        _connection.On("Pong", () =>
        {
            TimeSpan diff = DateTime.Now - lastPing;
            Pong?.Invoke(this, new PongEventArgs(diff.Milliseconds));
        });

        _pingTimer.Tick += PingTimer_Tick;
        _pingTimer.Interval = TimeSpan.FromSeconds(PingIntervalSeconds);
        _pingTimer.Start();
    }

    private Task Connection_Closed(Exception arg)
    {
        if (arg != null)
        {
            Disconnected.Invoke(this, null);
        }

        return Task.CompletedTask;
    }

    private Task Connection_Reconnected(string arg)
    {
        Reconnected.Invoke(this, null);
        return Task.CompletedTask;
    }

    private async void PingTimer_Tick(object sender, EventArgs e)
    {
        lastPing = DateTime.Now;
        await _connection.InvokeAsync("Ping");
    }
}

public class UserEventArgs : EventArgs
{
    public UserEventArgs(string user)
    {
        User = user;
    }

    public string User { get; private set; }
}

public class DrewEventArgs : EventArgs
{
    public DrewEventArgs(string user, byte[] data)
    {
        User = user;
        Data = data;
    }

    public string User { get; private set; }
    public byte[] Data { get; private set; }
}

public class PongEventArgs : EventArgs
{
    public PongEventArgs(int latency)
    {
        Latency = latency;
    }

    public int Latency { get; private set; }
}
