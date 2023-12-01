using EventManagementService.Application.V1.FetchEventById.Exceptions;
using EventManagementService.Application.V1.FetchEventById.Repositories;
using EventManagementService.Domain.Models.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventManagementService.Application.V1.FetchEventById;

public record FetchEventByIdRequest(int EventId): IRequest<Event>;

public class FetchEventByIdHandler: IRequestHandler<FetchEventByIdRequest, Event>
{
    private readonly ILogger<FetchEventByIdHandler> _logger;
    private readonly IEventRepository _eventRepository;
    private readonly IUserRepository _userRepository;
    
    public FetchEventByIdHandler(ILogger<FetchEventByIdHandler> logger, IEventRepository eventRepository, IUserRepository userRepository)
    {
        _logger = logger;
        _eventRepository = eventRepository;
        _userRepository = userRepository;
    }
    
    public async Task<Event> Handle(FetchEventByIdRequest request, CancellationToken cancellationToken)
    {
        var existingEvent = await _eventRepository.GetEventByIdAsync(request.EventId);
        if (existingEvent is null)
        {
            throw new EventNotFoundException(request.EventId);
        }

        var host = await _userRepository.GetUserById(existingEvent.Host.UserId);
        existingEvent.Host = host!;

        var attendees = await _userRepository.GetUsersAsync(existingEvent.Attendees.Select(a => a.UserId).ToList());
        existingEvent.Attendees = attendees;
        
        return existingEvent;
    }
}