using EventManagementService.Domain.Models;
using EventManagementService.Infrastructure.AppSettings;
using Google.Cloud.PubSub.V1;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EventManagementService.Application.ScraperEvents.Repository;

public interface IPubSubScraperEvents
{
    Task PublishScraperEvents(TopicName topicName, IReadOnlyCollection<Event> events);
    Task<IReadOnlyCollection<Event>> FetchScraperEvents(TopicName topicName);
}

public class PubSubScraperEvents : IPubSubScraperEvents
{
    private readonly ILogger<PubSubScraperEvents> _logger;
    private readonly IOptions<PubSub> _options;

    public PubSubScraperEvents
    (
        ILogger<PubSubScraperEvents> logger,
        IOptions<PubSub> options
    )
    {
        _logger = logger;
        _options = options;
    }

    public Task PublishScraperEvents
    (
        TopicName topicName,
        IReadOnlyCollection<Event> events
    )
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<Event>> FetchScraperEvents
    (
        TopicName topicName
    )
    {
        throw new NotImplementedException();
    }
}