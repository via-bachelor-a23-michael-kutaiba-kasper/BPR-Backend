using EventManagementService.Domain.Models.Events;
using Google.Apis.Logging;
using Microsoft.Extensions.Logging;
using RecommendationService.Domain.Util;
using RecommendationService.Infrastructure.ApiGateway;

namespace RecommendationService.Application.V1.GetRecommendations.Repository;

public interface IEventsRepository
{
    public Task<IReadOnlyCollection<Event>> GetEventsWhereUserHasAttendedAsync(string userId);
    public Task<IReadOnlyCollection<Event>> GetAllEvents(DateTimeOffset? from = null);
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
        var response = await _apiGateway.QueryAsync<IReadOnlyCollection<Event>>(new ApiGatewayQuery
        {
            Query = FinishedJoinedEventsQuery,
            Variables = new {userId}
        }, "finishedJoinedEvents");

        return response.Result;
    }

    public async Task<IReadOnlyCollection<Event>> GetAllEvents(DateTimeOffset? from = null)
    {
        var response = await _apiGateway.QueryAsync<IReadOnlyCollection<Event>>(new ApiGatewayQuery
        {
            Query = EventsQuery,
            Variables = new
            {
                from = from.HasValue ? from.Value.ToFormattedUtcString() : DateTimeOffset.UtcNow.ToFormattedUtcString()
            }
        },"events");

        return response.Result;
    }

    private string FinishedJoinedEventsQuery => """
            query($userId: String){
             finishedJoinedEvents(userId: $userId) {
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
                message
                code
              }
            }
            }
""";

    private string EventsQuery => """
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