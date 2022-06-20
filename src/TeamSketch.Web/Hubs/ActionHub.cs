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
    private readonly ILiveViewService _liveViewService;

    public ActionHub(IRepository repository, IRandomRoomQueue randomRoomQueue, ILiveViewService liveViewService)
    {
        _repository = repository;
        _randomRoomQueue = randomRoomQueue;
        _liveViewService = liveViewService;
    }

    public async Task CreateRoom(string nickname)
    {
        var nicknameError = Validations.ValidateNickname(nickname);
        if (nicknameError != null)
        {
            throw new InvalidOperationException(nicknameError);
        }

        string room = RoomNameGenerator.Generate();

        await Groups.AddToGroupAsync(Context.ConnectionId, room);
        await _repository.CreateRoomAsync(room, false, nickname, Context.ConnectionId, GetIPAddress());

        await Clients.Caller.SendAsync("RoomCreated", room);
    }

    public async Task JoinRoom(string nickname, string room)
    {
        var nicknameError = Validations.ValidateNickname(nickname);
        if (nicknameError != null)
        {
            throw new InvalidOperationException(nicknameError);
        }

        var exists = await _repository.RoomExistsAsync(room);
        if (!exists)
        {
            throw new InvalidOperationException($"Room '{room}' does not exist.");
        }

        var participantsInRoom = await _repository.GetActiveParticipantsInRoomAsync(room);
        if (participantsInRoom.Count > 4)
        {
            throw new InvalidOperationException($"Room '{room}' is currently full.");
        }

        if (participantsInRoom.Contains(nickname))
        {
            throw new InvalidOperationException($"Nickname '{nickname}' is taken in room '{room}'.");
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, room);
        await _repository.JoinRoomAsync(room, nickname, Context.ConnectionId, GetIPAddress());

        await Clients.OthersInGroup(room).SendAsync("JoinedRoom", nickname);
    }

    public async Task JoinRandomRoom(string nickname)
    {
        UserInQueue? userInQueue = _randomRoomQueue.Dequeue();
        if (userInQueue == null)
        {
            _randomRoomQueue.Enqueue(Context.ConnectionId, nickname, GetIPAddress());
            return;
        }

        string room = RoomNameGenerator.Generate();

        await Groups.AddToGroupAsync(userInQueue.ConnectionId, room);
        await Groups.AddToGroupAsync(Context.ConnectionId, room);

        await _repository.CreateRoomAsync(room, true, userInQueue.Nickname, userInQueue.ConnectionId, userInQueue.IpAddress);

        await _repository.JoinRoomAsync(room, userInQueue.Nickname, userInQueue.ConnectionId, userInQueue.IpAddress);
        var ipAddress = GetIPAddress();
        await _repository.JoinRoomAsync(room, nickname, Context.ConnectionId, ipAddress);

        await Clients.Group(room).SendAsync("RandomRoomJoined", room);

        await _liveViewService.AddAsync(userInQueue.ConnectionId, userInQueue.IpAddress);
        await _liveViewService.AddAsync(Context.ConnectionId, ipAddress);
    }

    public async Task DrawPoint(string nickname, string room, byte[] data)
    {
        await Clients.OthersInGroup(room).SendAsync("DrewPoint", nickname, data);
    }

    public async Task DrawLine(string nickname, string room, byte[] data)
    {
        await Clients.OthersInGroup(room).SendAsync("DrewLine", nickname, data);
    }

    public Task Ping()
    {
        return Clients.Caller.SendAsync("Pong");
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _randomRoomQueue.Remove(Context.ConnectionId);

        var connectionRoom = await _repository.DisconnectAsync(Context.ConnectionId);
        if (connectionRoom != null)
        {
            await Clients.OthersInGroup(connectionRoom.Room).SendAsync("LeftRoom", connectionRoom.Nickname);
        }

        _liveViewService.Remove(Context.ConnectionId);

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
