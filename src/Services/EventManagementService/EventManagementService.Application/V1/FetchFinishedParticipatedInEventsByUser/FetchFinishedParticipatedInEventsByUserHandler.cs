using EventManagementService.Application.V1.FetchFinishedParticipatedInEventsByUser.Exceptions;
using EventManagementService.Application.V1.FetchFinishedParticipatedInEventsByUser.Repository;
using EventManagementService.Application.V1.FetchReviewsByUser.Exceptions;
using EventManagementService.Domain.Models.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using InvalidUserIdException = EventManagementService.Application.V1.FetchFinishedParticipatedInEventsByUser.Exceptions.InvalidUserIdException;

namespace EventManagementService.Application.V1.FetchFinishedParticipatedInEventsByUser;

public record FetchFinishedParticipatedInEventsByUserRequest(string UserId) : IRequest<IReadOnlyCollection<Event>>;

public class FetchFinishedParticipatedInEventsByUserHandler :
    IRequestHandler<FetchFinishedParticipatedInEventsByUserRequest, IReadOnlyCollection<Event>>
{
    private readonly ISqlEvent _sqlEvent;
    private readonly ILogger<FetchFinishedParticipatedInEventsByUserHandler> _logger;

    public FetchFinishedParticipatedInEventsByUserHandler
    (
        ISqlEvent sqlEvent,
        ILogger<FetchFinishedParticipatedInEventsByUserHandler> logger
    )
    {
        _sqlEvent = sqlEvent;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<Event>> Handle
    (
        FetchFinishedParticipatedInEventsByUserRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            if (string.IsNullOrEmpty(request.UserId) || string.IsNullOrWhiteSpace(request.UserId))
                throw new InvalidUserIdException("The user id must not be null");
            var events = await _sqlEvent.FetchFinishedParticipatedEventsByUserId(request.UserId);

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