using EventManagementService.Application.FetchAllEvents.Model;
using EventManagementService.Application.FetchAllEvents.Repository;
using EventManagementService.Domain.Models.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventManagementService.Application.FetchAllEvents;

public record FetchAllEventsRequest
(
    Filters Filters
) : IRequest<IReadOnlyCollection<Event>>;

public class FetchAllEventsHandler : IRequestHandler<FetchAllEventsRequest, IReadOnlyCollection<Event>>
{
    private readonly ISqlAllEvents _sqlAllEvents;
    private readonly ILogger<FetchAllEventsHandler> _logger;

    public FetchAllEventsHandler
    (
        ISqlAllEvents sqlAllEvents,
        ILogger<FetchAllEventsHandler> logger
    )
    {
        _sqlAllEvents = sqlAllEvents;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<Event>> Handle
    (
        FetchAllEventsRequest request,
        CancellationToken cancellationToken
    )
    {
        return await AllEvents(request.Filters);
    }

    private async Task<IReadOnlyCollection<Event>> AllEvents(Filters filters)
    {
        return await _sqlAllEvents.GetAllEvents(filters);
    }
}