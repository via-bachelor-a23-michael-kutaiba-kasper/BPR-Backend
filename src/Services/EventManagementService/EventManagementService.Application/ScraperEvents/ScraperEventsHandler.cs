using EventManagementService.Application.ScraperEvents.Repository;
using EventManagementService.Domain.Models;
using EventManagementService.Domain.Models.Events;
using EventManagementService.Infrastructure.AppSettings;
using Google.Cloud.PubSub.V1;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EventManagementService.Application.ScraperEvents;

public record ScraperEventsRequest
(
    TopicName TopicName, SubscriptionName SubscriptionName
) : IRequest<IReadOnlyCollection<Event>>;

public class ScraperEventsHandler : IRequestHandler<ScraperEventsRequest, IReadOnlyCollection<Event>>
{
    private readonly IPubSubScraperEvents _pubSubScraperEvents;
    private readonly ISqlScraperEvents _sqlScraperEvents;
    private readonly ILogger<ScraperEventsHandler> _logger;

    public ScraperEventsHandler
    (
        IPubSubScraperEvents pubSubScraperEvents,
        ISqlScraperEvents sqlScraperEvents,
        ILogger<ScraperEventsHandler> logger
    )
    {
        _pubSubScraperEvents = pubSubScraperEvents;
        _sqlScraperEvents = sqlScraperEvents;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<Event>> Handle
    (
        ScraperEventsRequest request,
        CancellationToken cancellationToken
    )
    {
        var events =
            await _pubSubScraperEvents.FetchEvents(request.TopicName, request.SubscriptionName, cancellationToken);

        await _sqlScraperEvents.UpsertEvents(events);

        return events;
    }
}