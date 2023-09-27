using System.Text.Json;
using Google.Api.Gax;
using Google.Cloud.PubSub.V1;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// poc pubsub 
var env = System.Environment.GetEnvironmentVariable("PUBSUB_EMULATOR_HOST");
Console.WriteLine(env);

Console.WriteLine("Test start");
// Use EmulatorDetection.EmulatorOrProduction to create service clients that will
// that will connect to the PubSub emulator if the PUBSUB_EMULATOR_HOST environment
// variable is set, but will otherwise connect to the production environment.

// Create the PublisherServiceApiClient using the PublisherServiceApiClientBuilder
// and setting the EmulatorDection property.
// Use the client as you'd normally do, to create a topic in this example.
TopicName topicName = new TopicName("pubsubtest", "test");

PublisherServiceApiClient publisherService = await new PublisherServiceApiClientBuilder
{
EmulatorDetection = EmulatorDetection.EmulatorOrProduction
}.BuildAsync();

// Use the client as you'd normally do, to create a topic in this example.
try
{

    publisherService.CreateTopic(topicName);
}
catch(Exception e)
{
    Console.WriteLine(e.Message);
}
// Create the SubscriberServiceApiClient using the SubscriberServiceApiClientBuilder
// and setting the EmulatorDection property.
SubscriberServiceApiClient subscriberService = await new SubscriberServiceApiClientBuilder
{
    EmulatorDetection = EmulatorDetection.EmulatorOrProduction
}.BuildAsync();

// Use the client as you'd normally do, to create a subscription in this example.
SubscriptionName subscriptionName = new SubscriptionName("pubsubtest", "testsub");
try
{ 
    subscriberService.CreateSubscription(subscriptionName, topicName, pushConfig: null, ackDeadlineSeconds: 60);
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}
Console.WriteLine(subscriberService.GetSubscription(subscriptionName).Topic);

// Create the SubscriberClient using SubscriberClientBuild to set the EmulatorDetection property.
SubscriberClient subscriber = await new SubscriberClientBuilder
{
    SubscriptionName = subscriptionName,
    EmulatorDetection = EmulatorDetection.EmulatorOrProduction
}.BuildAsync();

List<PubsubMessage> receivedMessages = new List<PubsubMessage>();

// Use the client as you'd normally do, to listen for messages in this example.
await subscriber.StartAsync((msg, cancellationToken) =>
{
    Console.WriteLine($"HERE {JsonSerializer.Serialize(msg)}");
    receivedMessages.Add(msg);
    Console.WriteLine($"Received message {msg.MessageId} published at {msg.PublishTime.ToDateTime()}");
    Console.WriteLine($"Text: '{msg.Data.ToStringUtf8()}'");
    // In this example we stop the subscriber when the message is received.
    // You may leave the subscriber running, and it will continue to received published messages
    // if any.
    // This is non-blocking, and the returned Task may be awaited.
    subscriber.StopAsync(TimeSpan.FromSeconds(15));
    // Return Reply.Ack to indicate this message has been handled.
    return Task.FromResult(SubscriberClient.Reply.Ack);
});
//

var response = subscriberService.Pull(subscriptionName, maxMessages: 10);
foreach (ReceivedMessage received in response.ReceivedMessages)
{
    PubsubMessage msg = received.Message;
    Console.WriteLine($"Received message {msg.MessageId} published at {msg.PublishTime.ToDateTime()}");
    Console.WriteLine($"Text: '{msg.Data.ToStringUtf8()}'");
}
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();