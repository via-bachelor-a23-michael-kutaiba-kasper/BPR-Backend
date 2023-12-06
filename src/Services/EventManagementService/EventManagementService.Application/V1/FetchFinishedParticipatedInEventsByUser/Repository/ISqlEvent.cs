using Dapper;
using EventManagementService.Application.V1.FetchFinishedParticipatedInEventsByUser.Data;
using EventManagementService.Application.V1.FetchFinishedParticipatedInEventsByUser.Exceptions;
using EventManagementService.Application.V1.FetchFinishedParticipatedInEventsByUser.Util;
using EventManagementService.Domain.Models;
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
            List<Event> events = new();
            
            var eventEntities = await connection.QueryAsync<EventEntity>
            (
                GetFinishedParticipatedEventsByUserIdSql,
                new { urId = userId, now = DateTimeOffset.UtcNow.ToUniversalTime() }
            ) ?? new List<EventEntity>();
            
            var eventIds = eventEntities.Select(e => e.id);
            var indexedKeywords = await GetIndexedKeywordsByEventId(connection, eventIds.ToList());
            var eventAttendeeEntities = eventIds.Any() ?  await connection.QueryAsync<EventAttendeeEntity>(
                    $"SELECT * FROM postgres.public.event_attendee WHERE event_id IN {SqlUtil.AsIntList(eventIds.ToList())}") :
                new List<EventAttendeeEntity>();
            var indexedImages = await GetIndexedImagesByEventId(connection, eventIds.ToList());
            var domainEvents = eventEntities.Select(e => new Event
            {
                Id = e.id,
                Category = (Category)e.category_id,
                City = e.city,
                Description = e.description,
                Host = new User() { UserId = e.host_id },
                Title = e.title,
                Url = e.url,
                AdultsOnly = e.adult_only,
                Location = e.location,
                StartDate = e.start_date,
                EndDate = e.end_date,
                AccessCode = e.access_code,
                IsPaid = e.is_paid,
                IsPrivate = e.is_private,
                CreatedDate = e.created_date,
                GeoLocation = new GeoLocation()
                {
                    Lat = e.geolocation_lat,
                    Lng = e.geolocation_lng
                },
                Images = indexedImages.TryGetValue(e.id, out var uri) ? uri : new List<string>(),
                LastUpdateDate = e.last_update_date,
                MaxNumberOfAttendees = e.max_number_of_attendees,
                Keywords = indexedKeywords[e.id].Select(id => (Keyword)id),
                Attendees = eventAttendeeEntities.Where(ea => ea.event_id == e.id).Select(ea => ea.user_id).Select(id => new User { UserId = id })
            });
            events.AddRange(domainEvents);

            _logger.LogInformation($"{events.Count()} events have been successfully retrieved");
            return events;
        }
        catch (Exception e)
        {
            _logger.LogError($"Failed to query events for user with user id: {userId}, {e.StackTrace}");
            throw new CannotQueryFinishedParticipatedEventsUser("Unable to query events for user", e);
        }
    }
    
    private async Task<IDictionary<int, List<int>>> GetIndexedKeywordsByEventId(NpgsqlConnection connection,
        IReadOnlyCollection<int> eventIds)
    {
        var queryEventKeywords =
            $"SELECT * from event_keyword WHERE event_id in {SqlUtil.AsIntList(eventIds.ToList())}";
        var eventKeywordEntities = eventIds.Any() ?  await connection.QueryAsync<EventKeywordEntity>(queryEventKeywords) :
            new List<EventKeywordEntity>();
        return IndexKeywordsByEventId(eventKeywordEntities.ToList());
    }
    
    
    private async Task<IDictionary<int, List<string>>> GetIndexedImagesByEventId(NpgsqlConnection connection,
        IReadOnlyCollection<int> eventIds)
    {
        var queryEventImages =
            $"SELECT * from image WHERE event_id in {SqlUtil.AsIntList(eventIds.ToList())}";
        var eventImages = eventIds.Any() ?  await connection.QueryAsync<EventImageEntity>(queryEventImages) :
            new List<EventImageEntity>();
        
        Dictionary<int, List<string>> indexedImages = new();
        foreach (var entity in eventImages)
        {
            if (!indexedImages.ContainsKey(entity.event_id))
            {
                indexedImages[entity.event_id] = new List<string>() {entity.uri};
                continue;
            }
            
            indexedImages[entity.event_id].Add(entity.uri);
        }

        return indexedImages;
    }
    
    private IDictionary<int, List<int>> IndexKeywordsByEventId(
        IReadOnlyCollection<EventKeywordEntity> eventKeywordEntities)
    {
        Dictionary<int, List<int>> indexedKeywords = new();

        foreach (var entity in eventKeywordEntities)
        {
            if (!indexedKeywords.ContainsKey(entity.event_id))
            {
                indexedKeywords[entity.event_id] = new List<int>() { entity.keyword };
            }
            else
            {
                indexedKeywords[entity.event_id].Add(entity.keyword);
            }
        }

        return indexedKeywords;
    }

    private const string GetFinishedParticipatedEventsByUserIdSql =
        """
        SELECT * FROM event e
            JOIN public.event_attendee ea on e.id = ea.event_id
                 WHERE ea.user_id = @urId AND e.end_date < @now
        """;
}