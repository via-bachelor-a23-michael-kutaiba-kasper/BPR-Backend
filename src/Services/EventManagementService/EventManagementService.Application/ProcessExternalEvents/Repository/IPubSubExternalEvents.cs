using System.Text.Json;
using EventManagementService.Application.ProcessExternalEvents.Exceptions;
using EventManagementService.Domain.Models.Events;
using EventManagementService.Infrastructure.AppSettings;
using Google.Api.Gax;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.PubSub.V1;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EventManagementService.Application.ProcessExternalEvents.Repository;

public interface IPubSubExternalEvents
{
    Task PublishEvents(TopicName topicName, IReadOnlyCollection<Event> events);

    Task<IReadOnlyCollection<Event>> FetchEvents(CancellationToken cancellationToken);
}

public class PubSubExternalEvents : IPubSubExternalEvents
{
    private readonly ILogger<PubSubExternalEvents> _logger;
    private readonly IOptions<PubSub> _options;
    private readonly string? _serviceAccountKeyJson;
    private readonly SubscriptionName? _subscriptionName;
    private readonly TopicName? _topicName;

    public PubSubExternalEvents
    (
        ILogger<PubSubExternalEvents> logger,
        IOptions<PubSub> options
    )
    {
        _logger = logger;
        _options = options;
        _serviceAccountKeyJson = Environment.GetEnvironmentVariable("SERVICE_ACCOUNT_KEY_JSON") ?? null;
        _subscriptionName = new SubscriptionName
        (
            _options.Value.Topics.First().ProjectId,
            _options.Value.SubscriptionName
        );
        _topicName = new TopicName
        (
            _options.Value.Topics.First().ProjectId,
            _options.Value.Topics.First().TopicId
        );
    }

    public async Task PublishEvents
    (
        TopicName topicName,
        IReadOnlyCollection<Event> events
    )
    {
        PublisherServiceApiClient publisherService;
        if (_serviceAccountKeyJson != null)
        {
            publisherService = await new PublisherServiceApiClientBuilder
            {
                EmulatorDetection = EmulatorDetection.EmulatorOrProduction,
                Credential = GoogleCredential.FromJson(_serviceAccountKeyJson)
            }.BuildAsync();
        }
        else
        {
            publisherService = await new PublisherServiceApiClientBuilder
            {
                EmulatorDetection = EmulatorDetection.EmulatorOrProduction
            }.BuildAsync();
        }

// Use the client as you'd normally do, to create a topic in this example.
        try
        {
            await publisherService.CreateTopicAsync(topicName);
        }
        catch (Exception e)
        {
            throw new PubSubPublisherException($"Cannot publish message: {e.Message}", e);
        }
    }

    public async Task<IReadOnlyCollection<Event>> FetchEvents(CancellationToken cancellationToken)
    {
        var events = new List<Event>();

        SubscriberServiceApiClient subscriber;

        if (_serviceAccountKeyJson != null)
        {
            subscriber = await new SubscriberServiceApiClientBuilder
            {
                EmulatorDetection = EmulatorDetection.EmulatorOrProduction,
                Credential = GoogleCredential.FromJson(_serviceAccountKeyJson)
            }.BuildAsync(cancellationToken);
        }
        else
        {
            subscriber = await new SubscriberServiceApiClientBuilder
            {
                EmulatorDetection = EmulatorDetection.EmulatorOrProduction
            }.BuildAsync(cancellationToken);
        }


        try
        {
            await subscriber.CreateSubscriptionAsync(_subscriptionName, _topicName, pushConfig: null,
                ackDeadlineSeconds: 60);
        }
        catch (Exception e)
        {
            _logger.LogInformation("Sub already created");
        }

        //subscriptionName, maxMessages: 10, returnImmediately: true
        var response = await subscriber.PullAsync(new PullRequest
        {
            SubscriptionAsSubscriptionName = _subscriptionName,
            MaxMessages = 10
        });
        foreach (var received in response.ReceivedMessages)
        {
            var msg = received.Message;
            Console.WriteLine(msg.Data.ToStringUtf8());
            _logger.LogInformation(msg.Data.ToStringUtf8());
            events.AddRange(JsonSerializer.Deserialize<List<Event>>(msg.Data.ToStringUtf8(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!);
            Console.WriteLine(events.ToString());
        }

        if (response.ReceivedMessages.Count > 0)
        {
            await subscriber.AcknowledgeAsync(_subscriptionName, response.ReceivedMessages.Select(m => m.AckId));
        }

        // NOTE: Use same subscription every time, since there is no guarantees that 
        //       messages published BEFORE subscription will be put into the new subscription
        // await subscriber.DeleteSubscriptionAsync(subscriptionName);
        return events;
    }
}