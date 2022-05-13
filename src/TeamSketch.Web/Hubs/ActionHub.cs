using Microsoft.AspNetCore.SignalR;
using TeamSketch.Common;
using TeamSketch.Web.Persistence;
using TeamSketch.Web.Services;
using TeamSketch.Web.Utils;

namespace TeamSketch.Web.Hubs;

public class ActionHub : Hub
{
    private readonly IRepository _repository;
    private readonly IRandomRoomQueue _randomRoomQueue;

    public ActionHub(IRepository repository, IRandomRoomQueue randomRoomQueue)
    {
        _repository = repository;
        _randomRoomQueue = randomRoomQueue;
    }

    public async Task CreateRoom(string user)
    {
        var nicknameError = Validations.ValidateNickname(user);
        if (nicknameError != null)
        {
            throw new InvalidOperationException(nicknameError);
        }

        string room = RoomNameGenerator.Generate();

        await Groups.AddToGroupAsync(Context.ConnectionId, room);
        await _repository.CreateRoomAsync(room, false, user, Context.ConnectionId, GetIPAddress());

        await Clients.Caller.SendAsync("RoomCreated", room);
    }

    public async Task JoinRoom(string user, string room)
    {
        var nicknameError = Validations.ValidateNickname(user);
        if (nicknameError != null)
        {
            throw new InvalidOperationException(nicknameError);
        }

        var exists = await _repository.RoomExistsAsync(room);
        if (!exists)
        {
            throw new InvalidOperationException($"Room '{room}' does not exist.");
        }

        var usersInRoom = await _repository.GetActiveUsersInRoomAsync(room);
        if (usersInRoom.Count > 4)
        {
            throw new InvalidOperationException($"Room '{room}' is currently full.");
        }

        if (usersInRoom.Contains(user))
        {
            throw new InvalidOperationException($"Nickname '{user}' is taken in room '{room}'.");
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, room);
        await _repository.JoinRoomAsync(room, user, Context.ConnectionId, GetIPAddress());

        await Clients.OthersInGroup(room).SendAsync("JoinedRoom", user);
    }

    public async Task JoinRandomRoom(string user)
    {
        UserInQueue? userInQueue = _randomRoomQueue.Dequeue();
        if (userInQueue == null)
        {
            _randomRoomQueue.Enqueue(Context.ConnectionId, user, GetIPAddress());
            return;
        }

        string room = RoomNameGenerator.Generate();

        await Groups.AddToGroupAsync(userInQueue.ConnectionId, room);
        await Groups.AddToGroupAsync(Context.ConnectionId, room);

        await _repository.CreateRoomAsync(room, true, userInQueue.Nickname, userInQueue.ConnectionId, userInQueue.IpAddress);

        await _repository.JoinRoomAsync(room, userInQueue.Nickname, userInQueue.ConnectionId, userInQueue.IpAddress);
        await _repository.JoinRoomAsync(room, user, Context.ConnectionId, GetIPAddress());

        await Clients.Group(room).SendAsync("RandomRoomJoined", room);
    }

    public async Task DrawPoint(string user, string room, byte[] data)
    {
        await Clients.OthersInGroup(room).SendAsync("DrewPoint", user, data);
    }

    public async Task DrawLine(string user, string room, byte[] data)
    {
        await Clients.OthersInGroup(room).SendAsync("DrewLine", user, data);
    }

    public Task Ping()
    {
        return Clients.Caller.SendAsync("Pong");
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var connectionRoom = await _repository.DisconnectAsync(Context.ConnectionId);

        await Clients.OthersInGroup(connectionRoom.Room).SendAsync("LeftRoom", connectionRoom.User);

        await base.OnDisconnectedAsync(exception);
    }

    private string? GetIPAddress()
    {
        var httpContext = Context.GetHttpContext();
        if (httpContext == null)
        {
            return null;
        }

        return httpContext.Request.Headers["X-Forwarded-For"];
    }
}
