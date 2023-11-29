using Dapper;
using EventManagementService.Application.FetchAllEvents.Data;
using EventManagementService.Application.FetchAllEvents.Model;
using EventManagementService.Domain.Models;
using EventManagementService.Domain.Models.Events;
using EventManagementService.Infrastructure;
using EventManagementService.Infrastructure.AppSettings;
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
    private readonly IUserRepository _userRepository;

    public SqlAllEvents
    (
        IConnectionStringManager connectionStringManager,
        ILogger<SqlAllEvents> logger,
        IUserRepository userRepository
    )
    {
        _connectionStringManager = connectionStringManager;
        _logger = logger;
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyCollection<Event>> GetAllEvents(Filters filters)
    {
        List<Event> events = new();
        using (var connection = new NpgsqlConnection())
        {
            await connection.OpenAsync();
            var queryEventStatement = $"SELECT * from event {new ApplyFiltersInSql().Apply(filters)}";
            var queryParams = new
            {
                @from = filters.From,
                @to = filters.To,
                @hostId = filters.HostId
            };
            
            var eventEntities =
                await connection.QueryAsync<EventEntity>(SqlQueries.QueryAllFromEventTable, queryParams) ??
                new List<EventEntity>();

            var eventIds = eventEntities.Select(e => e.id);
            var indexedKeywords = await GetIndexedKeywordsByEventId(connection, eventIds.ToList());

            var eventAttendeeEntities = await connection.QueryAsync<EventAttendeeEntity>(
                $"SELECT * FROM postgres.public.event_attendee WHERE event_id IN {SqlUtil.AsIntList(eventIds.ToList())}") ?? new List<EventAttendeeEntity>();
            var hostIds = eventEntities.Select(e => e.host_id);
            var attendeeIds = eventAttendeeEntities.Select(e => e.user_id);
            var userIds = hostIds.Concat(attendeeIds);
            var indexedUsers = await GetIndexedUsersByUuid(connection, userIds.ToList());

            var domainEvents = eventEntities.Select(e => new Event
            {
                Id = e.id,
                Category = (Category)e.category_id,
                City = e.city,
                Description = e.description,
                Host = indexedUsers[e.host_id],
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
                LastUpdateDate = e.last_update_date,
                MaxNumberOfAttendees = e.max_number_of_attendees,
                Keywords = indexedKeywords[e.id].Select(id => (Keyword)id),
                Attendees = eventAttendeeEntities.Where(ea => ea.event_id == e.id).Select(ea => indexedUsers[ea.user_id])
                
            });
        }

        return events;
    }

    private async Task<IDictionary<int, List<int>>> GetIndexedKeywordsByEventId(NpgsqlConnection connection,
        IReadOnlyCollection<int> eventIds)
    {
            var queryEventKeywords = $"SELECT * from event_keyword WHERE event_id in {SqlUtil.AsIntList(eventIds.ToList())}";
            var eventKeywordEntities = await connection.QueryAsync<EventKeywordEntity>(queryEventKeywords) ?? new List<EventKeywordEntity>();
            return IndexKeywordsByEventId(eventKeywordEntities.ToList());
    }

    private async Task<IDictionary<string, User>> GetIndexedUsersByUuid(NpgsqlConnection connection, IReadOnlyCollection<string> userIds)
    {
        IDictionary<string, User> indexes = new Dictionary<string, User>();
        var users = await _userRepository.GetUsersAsync(userIds);
        foreach (var user in users)
        {
            if (indexes.ContainsKey(user.UserId))
            {
                continue;
            }

            indexes[user.UserId] = user;
        }

        return indexes;
    }
    
    private IDictionary<int, List<int>> IndexKeywordsByEventId(IReadOnlyCollection<EventKeywordEntity> eventKeywordEntities)
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