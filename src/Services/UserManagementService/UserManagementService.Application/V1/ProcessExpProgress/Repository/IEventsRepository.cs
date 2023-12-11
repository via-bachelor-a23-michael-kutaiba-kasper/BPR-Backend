using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using UserManagementService.Application.V1.ProcessExpProgress.Data;
using UserManagementService.Domain.Models;
using UserManagementService.Domain.Models.Events;
using UserManagementService.Infrastructure;
using UserManagementService.Infrastructure.ApiGateway;
using UserManagementService.Infrastructure.AppSettings;
using UserManagementService.Infrastructure.PubSub;
using UserManagementService.Infrastructure.Util;

namespace UserManagementService.Application.V1.ProcessExpProgress.Repository;

public interface IEventsRepository
{
    public Task<IReadOnlyCollection<Event>> GetNewlyCreatedEvents();
    public Task<Event> GetById(int eventId);
    public Task<int> GetHostedEventsCount(string userId);
}

public class EventsRepository : IEventsRepository
{
    private readonly ILogger<EventsRepository> _logger;
    private readonly IEventBus _eventBus;
    private readonly IApiGateway _apiGateway;
    private readonly IConnectionStringManager _connectionStringManager;
    private readonly IOptions<PubSub> _pubsubConfig;

    public EventsRepository(IEventBus eventBus, IApiGateway apiGateway,
        IConnectionStringManager connectionStringManager, IOptions<PubSub> pubsubConfig,
        ILogger<EventsRepository> logger)
    {
        _eventBus = eventBus;
        _apiGateway = apiGateway;
        _connectionStringManager = connectionStringManager;
        _pubsubConfig = pubsubConfig;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<Event>> GetNewlyCreatedEvents()
    {
        try
        {
            _logger.LogInformation("Pulling new events from Pub Sub");
            
            var topic = _pubsubConfig.Value.Topics[PubSubTopics.VibeVerseEventsNewEvent];
            var newEvents = (await _eventBus.PullAsync<Event>(topic.TopicId, topic.ProjectId,
                topic.SubscriptionNames[PubSubSubscriptionNames.Exp],
                1000, new CancellationToken())).ToList();
            
            _logger.LogInformation($"Retrieved {newEvents.Count} newly created events");

            return newEvents;
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to pull event created messages from PubSub");
            _logger.LogError(e.Message);
            _logger.LogError(e.StackTrace);
            return new List<Event>();
        }
    }

    public async Task<Event> GetById(int eventId)
    {
        var response = await _apiGateway.QueryAsync<ReadEventDto>(new ApiGatewayQuery
        {
            Query = EventQuery,
            Variables = new
            {
                event_id = eventId
            }
        }, "event");
        var e = response.Result;

        return new Event
        {
            Keywords = e.Keywords.Select(EnumExtensions.GetEnumValueFromDescription<Keyword>),
            Category = EnumExtensions.GetEnumValueFromDescription<Category>(e.Category),
            GeoLocation = new GeoLocation()
            {
                Lat = e.GeoLocation.Lat,
                Lng = e.GeoLocation.Lng
            },
            Description = e.Description,
            Attendees = e.Attendees.Select(a => new User()
            {
                CreationDate = a.CreationDate,
                DisplayName = a.DisplayName,
                PhotoUrl = a.PhotoUrl,
                UserId = a.UserId,
                LastSeenOnline = a.LastSeenOnline
            }),
            City = e.City,
            Host = new User()
            {
                CreationDate = e.Host.CreationDate,
                DisplayName = e.Host.DisplayName,
                PhotoUrl = e.Host.PhotoUrl,
                UserId = e.Host.UserId,
                LastSeenOnline = e.Host.LastSeenOnline
            },
            Id = e.Id,
            Images = e.Images,
            Location = e.Location,
            Title = e.Title,
            Url = e.Url,
            AccessCode = e.AccessCode,
            AdultsOnly = e.AdultsOnly,
            CreatedDate = e.CreatedDate,
            EndDate = e.EndDate,
            IsPaid = e.IsPaid,
            IsPrivate = e.IsPrivate,
            StartDate = e.StartDate,
            LastUpdateDate = e.LastUpdateDate,
            MaxNumberOfAttendees = e.MaxNumberOfAttendees
        };
    }

    public async Task<int> GetHostedEventsCount(string userId)
    {
        await using var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString());

        var latestStatsEntryQuery = """
        SELECT * FROM user_progress.user_stats_history WHERE user_id=@userId ORDER BY datetime DESC LIMIT 1; 
        """;
        var latestStatsEntry = await connection.QueryFirstOrDefaultAsync<UserStatsHistoryEntity>(latestStatsEntryQuery,
            new
            {
                @userId = userId
            }) ?? new UserStatsHistoryEntity
        {
            user_id = userId,
            events_hosted = 0,
            datetime = DateTimeOffset.UtcNow,
            id = -1,
            reviews_created = 0
        };

        return latestStatsEntry.events_hosted;
    }

    private string EventQuery => """
      query Event($eventId: Int) {
        event(eventId: $eventId) {
          result {
            id
            title
            startDate
            endDate
            createdDate
            lastUpdateDate
            isPrivate
            adultsOnly
            isPaid
            host {
              userId
              displayName
              photoUrl
              lastSeenOnline
              creationDate
            }
            maxNumberOfAttendees
            url
            description
            accessCode
            category
            keywords
            images
            attendees {
              userId
              displayName
              photoUrl
              lastSeenOnline
              creationDate
            }
            geoLocation {
              lat
              lng
            }
            city
            location
          }
          status {
            code
            message
          }
        }
      }
""";

    private class ReadUserDto
    {
        public string UserId { get; set; }
        public string? DisplayName { get; set; }
        public string? PhotoUrl { get; set; }
        public DateTimeOffset? LastSeenOnline { get; set; }
        public DateTimeOffset CreationDate { get; set; }
    }

    private class ReadGeolocationDto
    {
        public float Lat { get; set; }
        public float Lng { get; set; }
    }

    private class ReadEventDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset LastUpdateDate { get; set; }
        public bool IsPrivate { get; set; }
        public bool AdultsOnly { get; set; }
        public bool IsPaid { get; set; }
        public ReadUserDto Host { get; set; }
        public int MaxNumberOfAttendees { get; set; }
        public string Url { get; set; }
        public string? Description { get; set; }
        public string Location { get; set; }
        public string City { get; set; }
        public ReadGeolocationDto GeoLocation { get; set; }
        public string AccessCode { get; set; }
        public string Category { get; set; }
        public IEnumerable<string> Keywords { get; set; }
        public IEnumerable<string>? Images { get; set; }
        public IEnumerable<ReadUserDto>? Attendees { get; set; }
    }
}