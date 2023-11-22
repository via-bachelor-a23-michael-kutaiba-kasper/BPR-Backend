using EventManagementService.Application.CreateEvent.Exceptions;
using EventManagementService.Application.CreateEvent.Repository;
using EventManagementService.Domain.Models.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventManagementService.Application.CreateEvent;

public record CreateEventRequest(Event Event) : IRequest;

public class CreateEventHandler : IRequestHandler<CreateEventRequest>
{
    private readonly ISqlCreateEvent _sqlCreateEvent;
    private readonly ILogger<CreateEventHandler> _logger;

    public CreateEventHandler(ISqlCreateEvent sqlCreateEvent, ILogger<CreateEventHandler> logger)
    {
        _sqlCreateEvent = sqlCreateEvent;
        _logger = logger;
    }

    public async Task Handle(CreateEventRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _sqlCreateEvent.InsertEvent(request.Event);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new CreateEventException(
                $"Something went wrong while creating a new event at: {DateTimeOffset.UtcNow}", e);
        }
    }
}