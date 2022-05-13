using System.Collections.Concurrent;

namespace TeamSketch.Web.Services;

public interface IRandomRoomQueue
{
    void Enqueue(string connectionId, string nickname, string? ipAddress);
    UserInQueue? Dequeue();
}

public class RandomRoomQueue : IRandomRoomQueue
{
    private readonly ConcurrentQueue<UserInQueue> _queue = new();

    public void Enqueue(string connectionId, string nickname, string? ipAddress)
    {
        _queue.Enqueue(new UserInQueue(connectionId, nickname, ipAddress));
    }

    public UserInQueue? Dequeue()
    {
        _queue.TryDequeue(out UserInQueue? user);
        return user;
    }
}

public record UserInQueue(string ConnectionId, string Nickname, string? IpAddress);
