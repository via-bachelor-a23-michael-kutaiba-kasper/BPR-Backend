using System.Text.Json;
using Google.Api.Gax;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Microsoft.Extensions.Logging;

namespace UserManagementService.Infrastructure.PubSub;

public interface IEventBus
{
    Task PublishAsync<T>(string topicName,string projectName, T data);
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
        await publisherService.PublishAsync(topic, new[] { message });
    }
}