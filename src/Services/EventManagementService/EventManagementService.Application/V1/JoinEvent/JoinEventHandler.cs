using EventManagementService.Application.V1.JoinEvent.Exceptions;
using EventManagementService.Application.V1.JoinEvent.Repositories;
using EventManagementService.Domain.Models.Events;
using EventManagementService.Infrastructure.AppSettings;
using EventManagementService.Infrastructure.EventBus;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EventManagementService.Application.V1.JoinEvent;

public record JoinEventRequest(string UserId, int EventId) : IRequest;

public class JoinEventHandler : IRequestHandler<JoinEventRequest>
{
    private readonly IEventRepository _eventRepository;
    private readonly IInvitationRepository _invitationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IEventBus _eventBus;
    private readonly ILogger<JoinEventHandler> _logger;
    private readonly IOptions<PubSub> _pubsubConfig;

    public JoinEventHandler(ILogger<JoinEventHandler> logger, IEventRepository eventRepository,
        IInvitationRepository invitationRepository, IUserRepository userRepository, IEventBus eventBus,
        IOptions<PubSub> pubsubConfig)
    {
        _logger = logger;
        _eventRepository = eventRepository;
        _invitationRepository = invitationRepository;
        _userRepository = userRepository;
        _eventBus = eventBus;
        _pubsubConfig = pubsubConfig;
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

        if (existingEvent.Attendees != null && existingEvent.Attendees.Count() >= existingEvent.MaxNumberOfAttendees && existingEvent.MaxNumberOfAttendees < 0)
        {
            throw new MaximumAttendeesReachedException(existingEvent.Id, existingEvent.MaxNumberOfAttendees);
        }

        if (!await _userRepository.UserExistsAsync(request.UserId))
        {
            throw new UserNotFoundException(request.UserId);
        }

        var userAlreadyJoined = existingEvent.Attendees?.Any(attendee => attendee.UserId == request.UserId) ?? false;
        if (userAlreadyJoined)
        {
            throw new AlreadyJoinedException(request.UserId, request.EventId);
        }

        if (request.UserId == existingEvent.Host.UserId)
        {
            throw new UserIsAlreadyHostOfEventException(request.UserId, existingEvent.Id);
        }

        await AcceptInvitationIfInvited(request.UserId, request.EventId);

        await _eventRepository.AddAttendeeToEventAsync(request.UserId, request.EventId);
        await _eventBus.PublishAsync(_pubsubConfig.Value.Topics[PubSubTopics.VibeVerseEventsNewAttendee].TopicId, _pubsubConfig.Value.Topics[PubSubTopics.VibeVerseEventsNewAttendee].ProjectId,
            request);
        _logger.LogInformation($"User {request.UserId} has been added as attendee to event {request.EventId}");
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