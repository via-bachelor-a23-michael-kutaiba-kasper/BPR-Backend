using EventManagementService.Application.JoinEvent.Exceptions;
using EventManagementService.Application.JoinEvent.Repositories;
using EventManagementService.Domain.Models.Events;
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
    private readonly ILogger<JoinEventHandler> _logger;

    public JoinEventHandler(ILogger<JoinEventHandler> logger, IEventRepository eventRepository,
        IInvitationRepository invitationRepository, IUserRepository userRepository)
    {
        _logger = logger;
        _eventRepository = eventRepository;
        _invitationRepository = invitationRepository;
        _userRepository = userRepository;
    }

    public async Task Handle(JoinEventRequest request, CancellationToken cancellationToken)
    {
        var existingEvent = await _eventRepository.GetByIdAsync(request.EventId);
        if (existingEvent is null)
        {
            throw new EventNotFoundException(request.EventId);
        }

        if (!await _userRepository.UserExists(request.UserId))
        {
            throw new UserNotFoundException(request.UserId);
        }

        var attendees = await _eventRepository.GetAttendeesAsync(request.EventId);
        var userAlreadyJoined = attendees.Any(attendee => attendee == request.UserId);
        if (userAlreadyJoined)
        {
            throw new AlreadyJoinedException(request.UserId, request.EventId);
        }
        
        await AcceptInvitationIfInvited(request.UserId, request.EventId);

        await _eventRepository.AddAttendeeToEventAsync(request.UserId, request.EventId);
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