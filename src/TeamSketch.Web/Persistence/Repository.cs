using System.Data;
using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;
using TeamSketch.Web.Config;

namespace TeamSketch.Web.Persistence;

public interface IRepository
{
    Task CreateRoomAsync(string room, string user, string signalRConnectionId, string? ipAddress);
    Task<List<string>> JoinRoomAsync(string room, string user, string signalRConnectionId, string? ipAddress);
    Task<ConnectionRoom> DisconnectAsync(string signalRConnectionId);
}

public class Repository : IRepository
{
    private readonly string? _connectionString;

    public Repository(IOptions<DatabaseSettings> databaseSettings)
    {
        _connectionString = databaseSettings.Value.ConnectionString;
    }

    public async Task CreateRoomAsync(string room, string user, string signalRConnectionId, string? ipAddress)
    {
        var now = DateTime.UtcNow;

        using IDbConnection conn = OpenConnection();
        using var transaction = conn.BeginTransaction();

        await conn.ExecuteAsync(@"SET CONSTRAINTS ""FK_connections_rooms_room_id"", ""FK_events_rooms_room_id"", ""FK_events_connections_connection_id"" DEFERRED", null, transaction);

        var roomId = await conn.ExecuteScalarAsync<int>("INSERT INTO rooms (name, created) VALUES (@name, @created) RETURNING id",
            new { name = room, created = now }, transaction);

        var connectionId = await conn.ExecuteScalarAsync<int>(@"INSERT INTO connections (room_id, signalr_connection_id, ip_address, ""user"", is_connected, created) VALUES (@roomId, @signalRConnectionId, @ipAddress, @user, TRUE, @created) RETURNING id", 
            new { roomId, signalRConnectionId, ipAddress, user, created = now }, transaction);

        await conn.ExecuteAsync("INSERT INTO events (room_id, connection_id, type, occurred) VALUES (@roomId, @connectionId, @type, @occurred)",
            new { roomId, connectionId, type = EventType.Joined, occurred = now }, transaction);

        transaction.Commit();
    }

    public async Task<List<string>> JoinRoomAsync(string room, string user, string signalRConnectionId, string? ipAddress)
    {
        var now = DateTime.UtcNow;

        using IDbConnection conn = OpenConnection();
        using var transaction = conn.BeginTransaction();

        await conn.ExecuteAsync(@"SET CONSTRAINTS ""FK_connections_rooms_room_id"", ""FK_events_rooms_room_id"", ""FK_events_connections_connection_id"" DEFERRED", null, transaction);

        var roomId = await conn.QueryFirstAsync<int>("SELECT id FROM rooms WHERE name = @name", new { name = room });
        var usersInRoom = (await conn.QueryAsync<string>(@"SELECT ""user"" FROM connections WHERE room_id = @roomId AND is_connected", new { roomId })).ToList();

        var connectionId = await conn.QueryFirstOrDefaultAsync<int?>(@"SELECT id FROM connections WHERE signalr_connection_id = @signalRConnectionId", new { signalRConnectionId });
        if (connectionId.HasValue)
        {
            await conn.ExecuteScalarAsync<int>(@"UPDATE connections SET is_connected = TRUE WHERE id = @connectionId", new { connectionId }, transaction);
        }
        else
        {
            connectionId = await conn.ExecuteScalarAsync<int>(@"INSERT INTO connections (room_id, signalr_connection_id, ip_address, ""user"", is_connected, created) VALUES (@roomId, @signalRConnectionId, @ipAddress, @user, TRUE, @created) RETURNING id",
                new { roomId, signalRConnectionId, ipAddress, user, created = now }, transaction);
        }

        await conn.ExecuteAsync("INSERT INTO events (room_id, connection_id, type, occurred) VALUES (@roomId, @connectionId, @type, @occurred)",
            new { roomId, connectionId, type = EventType.Joined, occurred = now }, transaction);

        transaction.Commit();

        return usersInRoom;
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

        await conn.ExecuteScalarAsync<int>(@"UPDATE connections SET is_connected = FALSE WHERE id = @connectionId", new { connectionRoom.ConnectionId }, transaction);

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
