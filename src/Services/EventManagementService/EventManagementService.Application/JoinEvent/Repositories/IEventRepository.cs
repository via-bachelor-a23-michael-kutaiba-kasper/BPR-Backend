using Dapper;
using EventManagementService.Application.JoinEvent.Data;
using EventManagementService.Application.JoinEvent.Factory;
using EventManagementService.Domain.Models.Events;
using Npgsql;

namespace EventManagementService.Application.JoinEvent.Repositories;

public interface IEventRepository
{
    public Task<Event?> GetByIdAsync(int id);
    public Task<bool> AddAttendeeToEventAsync(string userId, int eventId);
}

public class EventRepository : IEventRepository
{
    private readonly IConnectionStringFactory _connectionStringFactory;

    public EventRepository(IConnectionStringFactory connectionStringFactory)
    {
        _connectionStringFactory = connectionStringFactory;
    }

    public async Task<Event?> GetByIdAsync(int id)
    {
        await using var connection = new NpgsqlConnection(_connectionStringFactory.GetConnectionString());
        await connection.OpenAsync();
        var queryParams = new { @eventId = id };
        var existingEvent= await connection.QueryFirstAsync<Event>(SqlQueries.GetEventById, queryParams);
        return existingEvent;
    }

    public async Task<bool> AddAttendeeToEventAsync(string userId, int eventId)
    {
        await using var connection = new NpgsqlConnection(_connectionStringFactory.GetConnectionString());
        await connection.OpenAsync();
        var queryParams = new
        {
            @userId = userId,
            @eventId = eventId,
        };
        var rowsAffected = await connection.ExecuteAsync(SqlQueries.AddAttendeeToEvent, queryParams);
         
        return rowsAffected > 0;
    }
}