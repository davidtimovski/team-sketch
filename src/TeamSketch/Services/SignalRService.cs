using System;
using System.Threading.Tasks;
using Avalonia.Threading;
using Common;
using Microsoft.AspNetCore.SignalR.Client;

namespace TeamSketch.Services;

public interface ISignalRService
{
    HubConnection Connection { get; }
    string Nickname { get; }
    string Room { get; }
    Task CreateRoomAsync(string userNickname);
    Task JoinRoomAsync(string userNickname, string joinedRoom);
    Task WaveAsync();
    Task LeaveAsync();
    Task DrawAsync(ShapeDto shape);

    event EventHandler<UserEventArgs> Waved;
    event EventHandler<UserEventArgs> Joined;
    event EventHandler<UserEventArgs> Left;
    event EventHandler<DrewPointEventArgs> DrewPoint;
    event EventHandler<DrewLineEventArgs> DrewLine;
    event EventHandler<PongEventArgs> Pong;
}

public class SignalRService : ISignalRService
{
    private readonly DispatcherTimer _pingTimer = new();
    private DateTime lastPing;

    public SignalRService()
    {
        Connection = new HubConnectionBuilder()
           .WithUrl("http://localhost:5206/ActionHub")
           .Build();
    }

    private async void PingTimer_Tick(object sender, EventArgs e)
    {
        lastPing = DateTime.Now;
        await Connection.InvokeAsync("Ping");
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

    public async Task LeaveAsync()
    {
        await Connection.InvokeAsync("LeaveRoom", Nickname, Room);
    }

    public async Task DrawAsync(ShapeDto shape)
    {
        switch (shape)
        {
            case PointDto point:
                await Connection.InvokeAsync("DrawPoint", Nickname, Room, point);
                break;
            case LineDto line:
                await Connection.InvokeAsync("DrawLine", Nickname, Room, line);
                break;
        }
    }

    public event EventHandler<UserEventArgs> Waved;
    public event EventHandler<UserEventArgs> Joined;
    public event EventHandler<UserEventArgs> Left;
    public event EventHandler<DrewPointEventArgs> DrewPoint;
    public event EventHandler<DrewLineEventArgs> DrewLine;
    public event EventHandler<PongEventArgs> Pong;

    private void InitializeHandlersAndPingTimer()
    {
        Connection.On<string>("Waved", (user) =>
        {
            Waved.Invoke(this, new UserEventArgs
            {
                User = user
            });
        });

        Connection.On<string>("JoinedRoom", async (user) =>
        {
            await WaveAsync();

            Joined.Invoke(this, new UserEventArgs
            {
                User = user
            });
        });

        Connection.On<string>("LeftRoom", (user) =>
        {
            Left.Invoke(this, new UserEventArgs
            {
                User = user
            });
        });

        Connection.On<string, PointDto>("DrewPoint", (user, point) =>
        {
            DrewPoint.Invoke(this, new DrewPointEventArgs
            {
                User = user,
                Point = point
            });
        });

        Connection.On<string, LineDto>("DrewLine", (user, line) =>
        {
            DrewLine.Invoke(this, new DrewLineEventArgs
            {
                User = user,
                Line = line
            });
        });

        Connection.On("Pong", () =>
        {
            TimeSpan diff = DateTime.Now - lastPing;

            Pong.Invoke(this, new PongEventArgs
            {
                Latency = diff.Milliseconds
            });
        });

        _pingTimer.Tick += PingTimer_Tick;
        _pingTimer.Interval = TimeSpan.FromSeconds(5);
        _pingTimer.Start();
    }
}

public class UserEventArgs : EventArgs
{
    public string User { get; init; }
}

public class DrewPointEventArgs : EventArgs
{
    public string User { get; init; }
    public PointDto Point { get; init; }
}

public class DrewLineEventArgs : EventArgs
{
    public string User { get; init; }
    public LineDto Line { get; init; }
}

public class PongEventArgs : EventArgs
{
    public int Latency { get; init; }
}
