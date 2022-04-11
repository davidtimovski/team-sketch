using Common;
using Microsoft.AspNetCore.SignalR;

namespace TeamSketch.Web.Hubs;

public class ActionHub : Hub
{
    public async Task Join(string user, string room)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, room);
        await Clients.OthersInGroup(room).SendAsync("Joined", user);
    }

    public Task Wave(string user, string room)
    {
        return Clients.OthersInGroup(room).SendAsync("Waved", user);
    }

    public async Task Leave(string user, string room)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, room);
        await Clients.OthersInGroup(room).SendAsync("Left", user);
    }

    public async Task Draw(string user, string room, object shape)
    {
        await Clients.OthersInGroup(room).SendAsync("Drew", user, shape);
    }

    public async Task DrawPoint(string user, string room, PointDto shape)
    {
        await Clients.OthersInGroup(room).SendAsync("DrewPoint", user, shape);
    }

    public async Task DrawLine(string user, string room, LineDto shape)
    {
        await Clients.OthersInGroup(room).SendAsync("DrewLine", user, shape);
    }

    public Task Ping()
    {
        return Clients.Caller.SendAsync("Pong");
    }
}
