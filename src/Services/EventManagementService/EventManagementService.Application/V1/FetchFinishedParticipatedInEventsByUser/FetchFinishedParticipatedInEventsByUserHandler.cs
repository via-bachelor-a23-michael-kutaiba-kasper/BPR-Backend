using EventManagementService.Application.V1.FetchFinishedParticipatedInEventsByUser.Exceptions;
using EventManagementService.Application.V1.FetchFinishedParticipatedInEventsByUser.Repository;
using EventManagementService.Domain.Models;
using EventManagementService.Domain.Models.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using InvalidUserIdException =
    EventManagementService.Application.V1.FetchFinishedParticipatedInEventsByUser.Exceptions.InvalidUserIdException;

namespace EventManagementService.Application.V1.FetchFinishedParticipatedInEventsByUser;

public record FetchFinishedParticipatedInEventsByUserRequest(string UserId) : IRequest<IReadOnlyCollection<Event>>;

public class FetchFinishedParticipatedInEventsByUserHandler :
    IRequestHandler<FetchFinishedParticipatedInEventsByUserRequest, IReadOnlyCollection<Event>>
{
    private readonly ISqlEvent _sqlEvent;
    private readonly IFirebaseUser _firebaseUser;
    private readonly ILogger<FetchFinishedParticipatedInEventsByUserHandler> _logger;

    public FetchFinishedParticipatedInEventsByUserHandler
    (
        ISqlEvent sqlEvent,
        ILogger<FetchFinishedParticipatedInEventsByUserHandler> logger,
        IFirebaseUser firebaseUser
    )
    {
        _sqlEvent = sqlEvent;
        _logger = logger;
        _firebaseUser = firebaseUser;
    }

    public async Task<IReadOnlyCollection<Event>> Handle
    (
        FetchFinishedParticipatedInEventsByUserRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            IDictionary<string, User> usersMap = new Dictionary<string, User>();
            if (string.IsNullOrEmpty(request.UserId) || string.IsNullOrWhiteSpace(request.UserId))
                throw new InvalidUserIdException("The user id must not be null");
            var events = await _sqlEvent.FetchFinishedParticipatedEventsByUserId(request.UserId);

            foreach (var ev in events)
            {
                var userIds = ev.Attendees
                    .Select(a => a.UserId)
                    .Concat(new List<string> { ev.Host.UserId });

                var nonMappedUserIds = userIds.Where(id => !usersMap.ContainsKey(id));
                var nonMappedUsers = await _firebaseUser.GetUsers(nonMappedUserIds.ToList());
                foreach (var user in nonMappedUsers)
                {
                    if (usersMap.ContainsKey(user.UserId))
                    {
                        continue;
                    }

                    usersMap[user.UserId] = user;
                }

                ev.Host = usersMap[ev.Host.UserId];
                ev.Attendees = ev.Attendees.Select(user => usersMap[user.UserId]);
            }

            _logger.LogInformation(
                $"{events.Count()} events have been successfully fetched for user {request.UserId} at {DateTimeOffset.UtcNow}");
            return events;
        }
        catch (Exception e)
        {
            _logger.LogError(
                $"Cannot fetch events for user with id {request.UserId} at : {DateTimeOffset.UtcNow}, {e.StackTrace}");
            throw new FetchFinishedParticipatedInEventsByUserException(
                "Something went wrong while fetching user participated and finished events", e);
        }
    }
}