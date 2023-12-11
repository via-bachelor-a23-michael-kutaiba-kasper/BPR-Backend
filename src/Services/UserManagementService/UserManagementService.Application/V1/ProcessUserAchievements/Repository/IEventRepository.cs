using Microsoft.Extensions.Logging;
using UserManagementService.Application.V1.ProcessUserAchievements.Dto;
using UserManagementService.Application.V1.ProcessUserAchievements.Exceptions;
using UserManagementService.Application.V1.ProcessUserAchievements.Mapper;
using UserManagementService.Domain.Models.Events;
using UserManagementService.Infrastructure.ApiGateway;

namespace UserManagementService.Application.V1.ProcessUserAchievements.Repository;

public interface IEventRepository
{
    Task<IReadOnlyCollection<Event>> FetchJoinedFinishedEventsByUserId(string userId);
}

public class EventRepository : IEventRepository
{
    private readonly ILogger<EventRepository> _logger;
    private readonly IApiGateway _apiGateway;

    public EventRepository(ILogger<EventRepository> logger, IApiGateway apiGateway)
    {
        _logger = logger;
        _apiGateway = apiGateway;
    }

    public async Task<IReadOnlyCollection<Event>> FetchJoinedFinishedEventsByUserId(string userId)
    {
        try
        {
            var response = await _apiGateway.QueryAsync<IReadOnlyCollection<EventDto>>(new ApiGatewayQuery
            {
                Query = GetJoinedFinishedEventsQuery,
                Variables = new { userId }
            }, "finishedJoinedEvents");
            var events = EventMappers.FromDtoToDomainEventMapper(response.Result);

            _logger.LogInformation(
                $"{events.Count} finished joined events have been successfully fetched for user with id {userId}");
            return events;
        }
        catch (Exception e)
        {
            _logger.LogError($"Failed to fetch finished joined events for user with id {userId}, {e.StackTrace}");
            throw new FailedToFetchEventsException("Unable to fetch events", e);
        }
    }

    private const string GetJoinedFinishedEventsQuery =
        """"
        query($userId: String) {
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
          }
        }

        """";
}