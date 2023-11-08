using EventManagementService.Application.FetchAllEvents.Repository;
using EventManagementService.Domain.Models.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventManagementService.Application.FetchAllEvents;

public record AllEventsRequest
(
) : IRequest<IReadOnlyCollection<Event>>;

public class AllPublicEventsHandler : IRequestHandler<AllEventsRequest, IReadOnlyCollection<Event>>
{
    private readonly ISqlAllEvents _sqlAllEvents;
    private readonly ILogger<AllPublicEventsHandler> _logger;

    public AllPublicEventsHandler
    (
        ISqlAllEvents sqlAllEvents,
        ILogger<AllPublicEventsHandler> logger
    )
    {
        _sqlAllEvents = sqlAllEvents;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<Event>> Handle
    (
        AllEventsRequest request,
        CancellationToken cancellationToken
    )
    {
        return await AllEvents();
    }

    private async Task<IReadOnlyCollection<Event>> AllEvents()
    {
        return await _sqlAllEvents.GetAllEvents();
    }

    
}