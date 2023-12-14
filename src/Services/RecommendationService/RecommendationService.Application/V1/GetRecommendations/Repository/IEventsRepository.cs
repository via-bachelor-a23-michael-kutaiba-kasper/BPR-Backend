using EventManagementService.Domain.Models;
using EventManagementService.Domain.Models.Events;
using Microsoft.Extensions.Logging;
using RecommendationService.Domain;
using RecommendationService.Domain.Events;
using RecommendationService.Domain.Util;
using RecommendationService.Infrastructure.ApiGateway;

namespace RecommendationService.Application.V1.GetRecommendations.Repository;

public interface IEventsRepository
{
    public Task<IReadOnlyCollection<Event>> GetEventsWhereUserHasAttendedAsync(string userId);
    public Task<IReadOnlyCollection<Event>> GetAllEventsAsync(DateTimeOffset? from = null);
}

public class EventsRepository : IEventsRepository
{
    private readonly ILogger<EventsRepository> _logger;
    private readonly IApiGateway _apiGateway;

    public EventsRepository(ILogger<EventsRepository> logger, IApiGateway apiGateway)
    {
        _logger = logger;
        _apiGateway = apiGateway;
    }

    public async Task<IReadOnlyCollection<Event>> GetEventsWhereUserHasAttendedAsync(string userId)
    {
        var response = await _apiGateway.QueryAsync<IEnumerable<ReadEventDto>>(new ApiGatewayQuery
        {
            Query = JoinedEventsQuery,
            Variables = new { userId = userId, eventState = "COMPLETED" }
        }, "joinedEvents");

        var response2 = await _apiGateway.QueryAsync<IEnumerable<ReadEventDto>>(new ApiGatewayQuery
        {
            Query = JoinedEventsQuery,
            Variables = new { userId = userId, eventState = "CURRENT" }
        }, "joinedEvents");

        return response.Result.Select(e => new Event()
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
            }).Concat(
                response2.Result.Select(e => new Event()
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
                })
            )
            .ToList();
    }

    public async Task<IReadOnlyCollection<Event>> GetAllEventsAsync(DateTimeOffset? from = null)
    {
        var response = await _apiGateway.QueryAsync<IEnumerable<ReadEventDto>>(new ApiGatewayQuery
        {
            Query = EventsQuery,
            Variables = new
            {
                from = from.HasValue ? from.Value.ToFormattedUtcString() : DateTimeOffset.UtcNow.ToFormattedUtcString()
            }
        }, "events");

        return response.Result.Select(e => new Event()
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
        }).ToList();
    }

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

    private static string JoinedEventsQuery => """
                                               query($userId: String!, $eventState: String!) {
                                                 joinedEvents(userId: $userId, eventState: $eventState) {
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
                                                 }
                                               }
                                               """;

    private static string EventsQuery => """
                                               query Events($from: String) {
                                                 events(from: $from) {
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
}