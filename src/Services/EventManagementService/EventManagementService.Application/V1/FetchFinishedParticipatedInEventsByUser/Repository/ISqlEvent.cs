using Dapper;
using EventManagementService.Application.V1.FetchFinishedParticipatedInEventsByUser.Exceptions;
using EventManagementService.Domain.Models.Events;
using EventManagementService.Infrastructure;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace EventManagementService.Application.V1.FetchFinishedParticipatedInEventsByUser.Repository;

public interface ISqlEvent
{
    Task<IReadOnlyCollection<Event>> FetchFinishedParticipatedEventsByUserId(string userId);
}

public class SqlEvent : ISqlEvent
{
    private readonly ILogger<SqlEvent> _logger;
    private readonly IConnectionStringManager _connectionStringManager;

    public SqlEvent
    (
        ILogger<SqlEvent> logger,
        IConnectionStringManager connectionStringManager
    )
    {
        _logger = logger;
        _connectionStringManager = connectionStringManager;
    }

    public async Task<IReadOnlyCollection<Event>> FetchFinishedParticipatedEventsByUserId(string userId)
    {
        await using var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString());
        await connection.OpenAsync();

        try
        {
            var query = await connection.QueryAsync<Event>
            (
                GetFinishedParticipatedEventsByUserIdSql,
                new { urId = userId, now = DateTimeOffset.UtcNow.ToUniversalTime() }
            );

            _logger.LogInformation($"{query.Count()} events have been successfully retrieved");
            return query.ToList();
        }
        catch (Exception e)
        {
            _logger.LogError($"Failed to query events for user with user id: {userId}, {e.StackTrace}");
            throw new CannotQueryFinishedParticipatedEventsUser("Unable to query events for user", e);
        }
    }

    private const string GetFinishedParticipatedEventsByUserIdSql =
        """
        SELECT * FROM event e
            JOIN public.event_attendee ea on e.id = ea.event_id
                 WHERE ea.user_id = @urId AND e.end_date < @now
        """;
}