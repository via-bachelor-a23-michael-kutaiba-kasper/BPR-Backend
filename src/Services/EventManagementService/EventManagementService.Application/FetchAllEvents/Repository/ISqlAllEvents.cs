using Dapper;
using EventManagementService.Application.FetchAllEvents.Data;
using EventManagementService.Application.FetchAllEvents.Exceptions;
using EventManagementService.Application.FetchAllEvents.Model;
using EventManagementService.Domain.Models;
using EventManagementService.Domain.Models.Events;
using EventManagementService.Infrastructure;
using EventManagementService.Infrastructure.AppSettings;
using EventManagementService.Infrastructure.Util;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;

namespace EventManagementService.Application.FetchAllEvents.Repository;

public interface ISqlAllEvents
{
    Task<IReadOnlyCollection<Event>> GetAllEvents(Filters filters);
}

public class SqlAllEvents : ISqlAllEvents
{
    private readonly IConnectionStringManager _connectionStringManager;
    private readonly ILogger<SqlAllEvents> _logger;

    public SqlAllEvents
    (
        IConnectionStringManager connectionStringManager,
        ILogger<SqlAllEvents> logger
    )
    {
        _connectionStringManager = connectionStringManager;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<Event>> GetAllEvents(Filters filters)
    {
        try
        {
            List<Event> events = new();
            using (var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString()))
            {
                await connection.OpenAsync();
                var queryEventStatement = $"SELECT * from event {new ApplyFiltersInSql().Apply(filters)}";
                var queryParams = new
                {
                    @from = filters.From,
                    @to = filters.To,
                    @hostId = filters.HostId,
                };

                var eventEntities =
                    await connection.QueryAsync<EventEntity>(queryEventStatement, queryParams) ??
                    new List<EventEntity>();

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
                    Attendees = eventAttendeeEntities.Select(ea => ea.user_id).Select(id => new User { UserId = id })
                });
                
                events.AddRange(domainEvents);
            }

            return events;
        }
        catch (Exception e)
        {
            _logger.LogError($"Failed to fetch events from database:\n{e.StackTrace}");
            throw new FailedToFetchEventsException(e);
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
}