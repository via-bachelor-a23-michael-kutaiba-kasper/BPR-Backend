using System.Text.Json;
using Google.Api.Gax;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace UserManagementService.Infrastructure.PubSub;

public interface IEventBus
{
    Task PublishAsync<T>(string topicName, string projectName, T data);

    Task<IEnumerable<T>> PullAsync<T>(string topicName, string projectName, string subscriptionName, int maxMessages,
        CancellationToken cancellationToken);
}

public class PubSubEventBus : IEventBus
{
    private readonly ILogger<PubSubEventBus> _logger;

    public PubSubEventBus(ILogger<PubSubEventBus> logger)
    {
        _logger = logger;
    }

    public async Task PublishAsync<T>(string topicName, string projectName, T data)
    {
        PublisherServiceApiClient publisherService;
        var serviceAccountKeyJson = Environment.GetEnvironmentVariable("SERVICE_ACCOUNT_KEY_JSON") ?? null;
        if (serviceAccountKeyJson != null)
        {
            publisherService = await new PublisherServiceApiClientBuilder
            {
                EmulatorDetection = EmulatorDetection.EmulatorOrProduction,
                Credential = GoogleCredential.FromJson(serviceAccountKeyJson)
            }.BuildAsync();
        }
        else
        {
            publisherService = await new PublisherServiceApiClientBuilder
            {
                EmulatorDetection = EmulatorDetection.EmulatorOrProduction
            }.BuildAsync();
        }

        try
        {
            await publisherService.CreateTopicAsync(topicName);
        }
        catch (Exception e)
        {
            // NOTE: If an exception is thrown, it is usually due to the topic already existing. 
            //       If the topic already exists, we just want to proceed.
            _logger.LogWarning(e.ToString());
        }

        var dataAsJson = JsonSerializer.Serialize(data, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        PubsubMessage message = new()
        {
            Data = ByteString.CopyFromUtf8(dataAsJson)
        };

        TopicName topic = new TopicName(projectName, topicName);
        await publisherService.PublishAsync(topic, new[] {message});
    }

    public async Task<IEnumerable<T>> PullAsync<T>(string topicName, string projectName, string subscriptionName,
        int maxMessages,
        CancellationToken cancellationToken)
    {
        SubscriberServiceApiClient subscriber;
        var topic = new TopicName(projectName, topicName);
        var subscription = new SubscriptionName(projectName, subscriptionName);

        var serviceAccountKeyJson = Environment.GetEnvironmentVariable("SERVICE_ACCOUNT_KEY_JSON") ?? null;
        if (serviceAccountKeyJson != null)
        {
            subscriber = await new SubscriberServiceApiClientBuilder
            {
                EmulatorDetection = EmulatorDetection.EmulatorOrProduction,
                Credential = GoogleCredential.FromJson(serviceAccountKeyJson)
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
           var newSub = await subscriber.CreateSubscriptionAsync(subscription, topic, pushConfig: null,
                ackDeadlineSeconds: 60);
           subscription = newSub?.SubscriptionName;
        }
        catch (Exception e)
        {
            _logger.LogInformation("Subscription already created");
        }

        var entities = new List<T>();
        var response = await subscriber.PullAsync(new PullRequest
        {
            SubscriptionAsSubscriptionName = subscription,
            MaxMessages = maxMessages
        });

        foreach (var receivedMessage in response.ReceivedMessages)
        {
            var message = receivedMessage.Message.Data.ToStringUtf8();
            JObject entityJObject = Deserialize<JObject>(message);
            var entity = entityJObject.ToObject<T>();
            if (entity != null)
            {
                entities.Add(entity);
            }
        }

        if (response.ReceivedMessages.Count > 0)
        {
            await subscriber.AcknowledgeAsync(subscription, response.ReceivedMessages.Select(m => m.AckId));
        }


        return entities;
    }


    private T Deserialize<T>(string serializedEntity)
    {
        // NOTE: Using Newtonsoft instead of System.Text.Json since it can handle generics
        DefaultContractResolver contractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        };
        return JsonConvert.DeserializeObject<T>(serializedEntity, new JsonSerializerSettings()
        {
            ContractResolver = contractResolver
        });
    }
}