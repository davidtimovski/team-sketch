using Microsoft.AspNetCore.SignalR;

namespace TeamSketch.Web.Hubs;

public class ActionHub : Hub
{
    public async Task CreateRoom()
    {
        string room = Guid.NewGuid().ToString();
        await Groups.AddToGroupAsync(Context.ConnectionId, room);
        await Clients.Caller.SendAsync("RoomCreated", room);
    }

    public async Task JoinRoom(string user, string room)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, room);
        await Clients.OthersInGroup(room).SendAsync("JoinedRoom", user);
    }

    public Task Wave(string user, string room)
    {
        return Clients.OthersInGroup(room).SendAsync("Waved", user);
    }

    public async Task LeaveRoom(string user, string room)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, room);
        await Clients.OthersInGroup(room).SendAsync("LeftRoom", user);
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
}
