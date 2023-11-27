using EventManagementService.Application.JoinEvent.Exceptions;
using EventManagementService.Application.JoinEvent.Repositories;
using EventManagementService.Domain.Models.Events;
using EventManagementService.Infrastructure.EventBus;
using Google.Apis.Logging;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventManagementService.Application.JoinEvent;

public record JoinEventRequest(string UserId, int EventId) : IRequest;

public class JoinEventHandler : IRequestHandler<JoinEventRequest>
{
    private readonly IEventRepository _eventRepository;
    private readonly IInvitationRepository _invitationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IEventBus _eventBus;
    private readonly ILogger<JoinEventHandler> _logger;
    private const string TopicName = "vibeverse_events_new_attendee";

    public JoinEventHandler(ILogger<JoinEventHandler> logger, IEventRepository eventRepository,
        IInvitationRepository invitationRepository, IUserRepository userRepository, IEventBus eventBus)
    {
        _logger = logger;
        _eventRepository = eventRepository;
        _invitationRepository = invitationRepository;
        _userRepository = userRepository;
        _eventBus = eventBus;
    }

    /// <summary>
    /// Adds user to attendee list of the event.
    /// If the user has already been invited, then the invitation will go from
    /// "PENDING" to "ACCEPTED"
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="EventNotFoundException">No event with the provided event id exists</exception>
    /// <exception cref="UserNotFoundException">No user with the provided id exists</exception>
    /// <exception cref="AlreadyJoinedException">User has already joined the event</exception>
    public async Task Handle(JoinEventRequest request, CancellationToken cancellationToken)
    {
        var existingEvent = await _eventRepository.GetByIdAsync(request.EventId);
        if (existingEvent is null)
        {
            throw new EventNotFoundException(request.EventId);
        }

        if (!await _userRepository.UserExistsAsync(request.UserId))
        {
            throw new UserNotFoundException(request.UserId);
        }

        var userAlreadyJoined = existingEvent.Attendees.Any(attendee => attendee == request.UserId);
        if (userAlreadyJoined)
        {
            throw new AlreadyJoinedException(request.UserId, request.EventId);
        }
        
        await AcceptInvitationIfInvited(request.UserId, request.EventId);

        await _eventRepository.AddAttendeeToEventAsync(request.UserId, request.EventId);
        await _eventBus.PublishAsync(TopicName, request);
    }

    private async Task AcceptInvitationIfInvited(string userId, int eventId)
    {
        var invitations = await _invitationRepository.GetInvitationsAsync(eventId);
        var pendingInvitationForUser = invitations
            .Where(invitation => invitation.Status == InvitationStatus.Pending)
            .FirstOrDefault(invitation => invitation.UserId == userId);

        if (pendingInvitationForUser is not null)
        {
            await _invitationRepository.AcceptInvitationAsync(pendingInvitationForUser);
        }
    }
}