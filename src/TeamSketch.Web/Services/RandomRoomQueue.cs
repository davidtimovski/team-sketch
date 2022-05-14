using System.Collections.Concurrent;

namespace TeamSketch.Web.Services;

public interface IRandomRoomQueue
{
    void Enqueue(string connectionId, string nickname, string? ipAddress);
    UserInQueue? Dequeue();
    void Remove(string connectionId);
}

public class RandomRoomQueue : IRandomRoomQueue
{
    private readonly ConcurrentDictionary<string, UserInQueue> _queue = new();

    public void Enqueue(string connectionId, string nickname, string? ipAddress)
    {
        _queue.TryAdd(connectionId, new UserInQueue(connectionId, nickname, ipAddress));
    }

    public UserInQueue? Dequeue()
    {
        if (_queue.IsEmpty)
        {
            return null;
        }

        _queue.Remove(_queue.First().Key, out UserInQueue? userInQueue);

        return userInQueue;
    }

    public void Remove(string connectionId)
    {
        _queue.TryRemove(connectionId, out UserInQueue? _);
    }
}

public record UserInQueue(string ConnectionId, string Nickname, string? IpAddress);
