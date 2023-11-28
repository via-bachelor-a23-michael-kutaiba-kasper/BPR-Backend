using System.Text.Json;
using EventManagementService.Application.FetchEventById.Exceptions;
using EventManagementService.Application.FetchEventById.Repositories;
using EventManagementService.Domain.Models.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventManagementService.Application.FetchEventById;

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
        
        return existingEvent;
    }
}