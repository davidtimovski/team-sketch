using System.Data;
using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;
using TeamSketch.Web.Config;

namespace TeamSketch.Web.Persistence;

public interface IRepository
{
    Task<bool> RoomExistsAsync(string room);
    Task<List<string>> GetActiveUsersInRoomAsync(string room);
    Task CreateRoomAsync(string room, bool isPublic, string user, string signalRConnectionId, string? ipAddress);
    Task JoinRoomAsync(string room, string user, string signalRConnectionId, string? ipAddress);
    Task<ConnectionRoom> DisconnectAsync(string signalRConnectionId);
}

public class Repository : IRepository
{
    private readonly string? _connectionString;

    public Repository(IOptions<DatabaseSettings> databaseSettings)
    {
        _connectionString = databaseSettings.Value.ConnectionString;
    }

    public async Task<bool> RoomExistsAsync(string room)
    {
        using IDbConnection conn = OpenConnection();

        return await conn.ExecuteScalarAsync<bool>(@"SELECT COUNT(*) FROM rooms WHERE name = @room", new { room });
    }

    public async Task<List<string>> GetActiveUsersInRoomAsync(string room)
    {
        using IDbConnection conn = OpenConnection();

        return (await conn.QueryAsync<string>(@"SELECT c.""user""
            FROM rooms AS r
            INNER JOIN connections AS c ON r.id = c.room_id AND c.is_connected
            WHERE r.name = @room", new { room })).ToList();
    }

    public async Task CreateRoomAsync(string room, bool isPublic, string user, string signalRConnectionId, string? ipAddress)
    {
        var now = DateTime.UtcNow;

        using IDbConnection conn = OpenConnection();
        using var transaction = conn.BeginTransaction();

        await conn.ExecuteAsync(@"SET CONSTRAINTS ""FK_connections_rooms_room_id"", ""FK_events_rooms_room_id"", ""FK_events_connections_connection_id"" DEFERRED", null, transaction);

        var roomId = await conn.ExecuteScalarAsync<int>("INSERT INTO rooms (name, is_public, created) VALUES (@name, @isPublic, @created) RETURNING id",
            new { name = room, isPublic, created = now }, transaction);

        var connectionId = await conn.ExecuteScalarAsync<int>(@"INSERT INTO connections (room_id, signalr_connection_id, ip_address, ""user"", is_connected, created, modified)
            VALUES (@roomId, @signalRConnectionId, @ipAddress, @user, TRUE, @created, @modified) RETURNING id", 
            new { roomId, signalRConnectionId, ipAddress, user, created = now, modified = now }, transaction);

        await conn.ExecuteAsync("INSERT INTO events (room_id, connection_id, type, occurred) VALUES (@roomId, @connectionId, @type, @occurred)",
            new { roomId, connectionId, type = EventType.Joined, occurred = now }, transaction);

        transaction.Commit();
    }

    public async Task JoinRoomAsync(string room, string user, string signalRConnectionId, string? ipAddress)
    {
        var now = DateTime.UtcNow;

        using IDbConnection conn = OpenConnection();
        using var transaction = conn.BeginTransaction();

        await conn.ExecuteAsync(@"SET CONSTRAINTS ""FK_connections_rooms_room_id"", ""FK_events_rooms_room_id"", ""FK_events_connections_connection_id"" DEFERRED", null, transaction);

        var roomId = await conn.QueryFirstAsync<int>("SELECT id FROM rooms WHERE name = @name", new { name = room });

        var connectionId = await conn.QueryFirstOrDefaultAsync<int?>(@"SELECT id FROM connections WHERE signalr_connection_id = @signalRConnectionId", new { signalRConnectionId });
        if (connectionId.HasValue)
        {
            await conn.ExecuteScalarAsync<int>(@"UPDATE connections SET is_connected = TRUE, modified = @modified WHERE id = @connectionId", new { connectionId, modified = now }, transaction);
        }
        else
        {
            connectionId = await conn.ExecuteScalarAsync<int>(@"INSERT INTO connections (room_id, signalr_connection_id, ip_address, ""user"", is_connected, created, modified)
                VALUES (@roomId, @signalRConnectionId, @ipAddress, @user, TRUE, @created, @modified) RETURNING id",
                new { roomId, signalRConnectionId, ipAddress, user, created = now, modified = now }, transaction);
        }

        await conn.ExecuteAsync("INSERT INTO events (room_id, connection_id, type, occurred) VALUES (@roomId, @connectionId, @type, @occurred)",
            new { roomId, connectionId, type = EventType.Joined, occurred = now }, transaction);

        transaction.Commit();
    }

    public async Task<ConnectionRoom> DisconnectAsync(string signalRConnectionId)
    {
        var now = DateTime.UtcNow;

        using IDbConnection conn = OpenConnection();
        using var transaction = conn.BeginTransaction();

        await conn.ExecuteAsync(@"SET CONSTRAINTS ""FK_events_rooms_room_id"", ""FK_events_connections_connection_id"" DEFERRED", null, transaction);

        var connectionRoom = await conn.QueryFirstAsync<ConnectionRoom>(@"SELECT c.id AS ""ConnectionId"", c.""user"", r.id AS ""RoomId"", r.name AS ""Room""
            FROM connections AS c
            INNER JOIN rooms AS r ON c.room_id = r.id
            WHERE signalr_connection_id = @signalRConnectionId", new { signalRConnectionId });

        await conn.ExecuteScalarAsync<int>(@"UPDATE connections SET is_connected = FALSE, modified = @modified WHERE id = @connectionId", new { connectionId = connectionRoom.ConnectionId, modified = now }, transaction);

        await conn.ExecuteAsync("INSERT INTO events (room_id, connection_id, type, occurred) VALUES (@roomId, @connectionId, @type, @occurred)",
            new { roomId = connectionRoom.RoomId, connectionId = connectionRoom.ConnectionId, type = EventType.Disconnected, occurred = now }, transaction);

        transaction.Commit();

        return connectionRoom;
    }

    private IDbConnection OpenConnection()
    {
        var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        return conn;
    }
}
