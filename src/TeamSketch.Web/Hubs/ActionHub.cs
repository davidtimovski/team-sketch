using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;
using TeamSketch.Common;
using TeamSketch.Web.Persistence;
using TeamSketch.Web.Utils;

namespace TeamSketch.Web.Hubs;

public class ActionHub : Hub
{
    private readonly IRepository _repository;

    public ActionHub(IRepository repository)
    {
        _repository = repository;
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
        await Clients.Caller.SendAsync("RoomCreated", room);

        await _repository.CreateRoomAsync(room, user, Context.ConnectionId, GetIPAddress());
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
        await Clients.OthersInGroup(room).SendAsync("JoinedRoom", user);

        await _repository.JoinRoomAsync(room, user, Context.ConnectionId, GetIPAddress());
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
        var httpConnection = Context.Features.Get<IHttpConnectionFeature>();
        return httpConnection?.RemoteIpAddress?.ToString();
    }
}
