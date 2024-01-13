namespace TeamSketch.Web.Persistence;

public class ConnectionRoom
{
    public required int ConnectionId { get; init; }
    public required string Nickname { get; init; }
    public required int RoomId { get; init; }
    public required string Room { get; init; }
}
