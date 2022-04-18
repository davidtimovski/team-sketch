using System;
using System.Threading.Tasks;
using Avalonia.Threading;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using TeamSketch.Models;

namespace TeamSketch.Services;

public interface ISignalRService
{
    HubConnection Connection { get; }
    string Nickname { get; }
    string Room { get; }
    Task CreateRoomAsync(string userNickname);
    Task JoinRoomAsync(string userNickname, string joinedRoom);
    Task WaveAsync();
    Task DisconnectAsync();
    Task DrawPointAsync(double x, double y, ThicknessEnum size, ColorsEnum color);
    Task DrawLineAsync(double x1, double y1, double x2, double y2, ThicknessEnum thickness, ColorsEnum color);

    event EventHandler<UserEventArgs> Waved;
    event EventHandler<UserEventArgs> Joined;
    event EventHandler<UserEventArgs> Left;
    event EventHandler<DrewEventArgs> DrewPoint;
    event EventHandler<DrewEventArgs> DrewLine;
    event EventHandler<PongEventArgs> Pong;
}

public class SignalRService : ISignalRService
{
    private const int PingIntervalSeconds = 4;
    private readonly DispatcherTimer _pingTimer = new();
    private DateTime lastPing;

    public SignalRService()
    {
        Connection = new HubConnectionBuilder()
#if DEBUG
           .WithUrl("http://localhost:5150/actionHub")
#else
           .WithUrl("https://team-sketch.davidtimovski.com/actionHub")
#endif
           .AddMessagePackProtocol()
           .Build();
    }

    public HubConnection Connection { get; }
    public string Nickname { get; private set; }
    public string Room { get; private set; }

    public async Task CreateRoomAsync(string nickname)
    {
        await Connection.StartAsync();

        Nickname = nickname.Trim();

        InitializeHandlersAndPingTimer();

        Connection.On<string>("RoomCreated", (room) =>
        {
            Room = room;
        });

        await Connection.InvokeAsync("CreateRoom");
    }

    public async Task JoinRoomAsync(string nickname, string room)
    {
        await Connection.StartAsync();

        Nickname = nickname.Trim();
        Room = room;

        InitializeHandlersAndPingTimer();

        await Connection.InvokeAsync("JoinRoom", Nickname, Room);
    }

    public async Task WaveAsync()
    {
        await Connection.InvokeAsync("Wave", Nickname, Room);
    }

    public async Task DisconnectAsync()
    {
        _pingTimer.Stop();
        await Connection.InvokeAsync("LeaveRoom", Nickname, Room);
        await Connection.StopAsync();
    }

    public async Task DrawPointAsync(double x, double y, ThicknessEnum size, ColorsEnum color)
    {
        var data = PayloadConverter.PointToBytes(x, y, size, color);
        await Connection.InvokeAsync("DrawPoint", Nickname, Room, data);
    }

    public async Task DrawLineAsync(double x1, double y1, double x2, double y2, ThicknessEnum thickness, ColorsEnum color)
    {
        var data = PayloadConverter.LineToBytes(x1, y1, x2, y2, thickness, color);
        await Connection.InvokeAsync("DrawLine", Nickname, Room, data);
    }

    public event EventHandler<UserEventArgs> Waved;
    public event EventHandler<UserEventArgs> Joined;
    public event EventHandler<UserEventArgs> Left;
    public event EventHandler<DrewEventArgs> DrewPoint;
    public event EventHandler<DrewEventArgs> DrewLine;
    public event EventHandler<PongEventArgs> Pong;

    private void InitializeHandlersAndPingTimer()
    {
        Connection.On<string>("Waved", (user) =>
        {
            Waved.Invoke(this, new UserEventArgs(user));
        });

        Connection.On<string>("JoinedRoom", async (user) =>
        {
            await WaveAsync();

            Joined.Invoke(this, new UserEventArgs(user));
        });

        Connection.On<string>("LeftRoom", (user) =>
        {
            Left.Invoke(this, new UserEventArgs(user));
        });

        Connection.On<string, byte[]>("DrewPoint", (user, data) =>
        {
            DrewPoint.Invoke(this, new DrewEventArgs(user, data));
        });

        Connection.On<string, byte[]>("DrewLine", (user, data) =>
        {
            DrewLine.Invoke(this, new DrewEventArgs(user, data));
        });

        Connection.On("Pong", () =>
        {
            TimeSpan diff = DateTime.Now - lastPing;
            Pong.Invoke(this, new PongEventArgs(diff.Milliseconds));
        });

        _pingTimer.Tick += PingTimer_Tick;
        _pingTimer.Interval = TimeSpan.FromSeconds(PingIntervalSeconds);
        _pingTimer.Start();
    }

    private async void PingTimer_Tick(object sender, EventArgs e)
    {
        lastPing = DateTime.Now;
        await Connection.InvokeAsync("Ping");
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
