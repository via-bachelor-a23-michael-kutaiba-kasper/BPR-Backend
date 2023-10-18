using EventManagementService.Domain.Models;
using MediatR;

namespace EventManagementService.Application.ScraperEvents;

public record ScraperEventsRequest() : IRequest<IReadOnlyCollection<Event>>;

public class ScraperEventsHandler : IRequestHandler<ScraperEventsRequest, IReadOnlyCollection<Event>>
{
    public Task<IReadOnlyCollection<Event>> Handle
    (
        ScraperEventsRequest request,
        CancellationToken cancellationToken
    )
    {
        throw new NotImplementedException();
    }
}