using Dapper;
using EventManagementService.Application.JoinEvent.Data;
using EventManagementService.Application.JoinEvent.Factory;
using EventManagementService.Domain.Models.Events;
using Npgsql;

namespace EventManagementService.Application.JoinEvent.Repositories;

public interface IEventRepository
{
    public Task<Event> GetByIdAsync(int id);
    public Task<bool> AddAttendeeToEventAsync(string userId, int eventId);
    public Task<IReadOnlyCollection<string>> GetAttendeesAsync(int eventId);
}

public class EventRepository : IEventRepository
{
    private readonly IConnectionStringFactory _connectionStringFactory;
    
    public EventRepository(IConnectionStringFactory connectionStringFactory)
    {
        _connectionStringFactory = connectionStringFactory;
    }
    
    public Task<Event> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> AddAttendeeToEventAsync(string userId, int eventId)
    {
        await using var connection = new NpgsqlConnection(_connectionStringFactory.GetConnectionString());
        await connection.OpenAsync();
        var rowsAffected = await connection.ExecuteAsync(SqlQueries.AddAttendeeToEvent, new { @userId = userId, @eventId = eventId });
        return rowsAffected > 0;
    }

    public async Task<IReadOnlyCollection<string>> GetAttendeesAsync(int eventId)
    {
        await using var connection = new NpgsqlConnection(_connectionStringFactory.GetConnectionString());
        await connection.OpenAsync();
        var attendees = await connection.QueryAsync<string>(SqlQueries.GetAttendeesOfEvent, new { @eventId = eventId });
        return attendees != null ? attendees.ToList() : new List<string>();
    }
}