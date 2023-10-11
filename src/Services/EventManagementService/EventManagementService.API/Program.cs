using System.Text.Json;
using Dapper;
using EventManagementService.Application.ScraperEvents;
using EventManagementService.Domain.Models;
using Google.Api.Gax;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.PubSub.V1;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IScraperEvents, ScraperEvents>();
var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseCors(options => { options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); });

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
Thread thread = new Thread(async () =>
{
    TopicName topicName = new TopicName("bachelorshenanigans", "test");

    var serviceAccountKeyJson = Environment.GetEnvironmentVariable("SERVICE_ACCOUNT_KEY_JSON");
    PublisherServiceApiClient publisherService = await new PublisherServiceApiClientBuilder
    {
        EmulatorDetection = EmulatorDetection.EmulatorOrProduction,
        Credential = GoogleCredential.FromJson(serviceAccountKeyJson)
    }.BuildAsync();

// Use the client as you'd normally do, to create a topic in this example.
    try
    {
        publisherService.CreateTopic(topicName);
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
    }

// Create the SubscriberServiceApiClient using the SubscriberServiceApiClientBuilder
// and setting the EmulatorDection property.
    SubscriberServiceApiClient subscriberService = await new SubscriberServiceApiClientBuilder
    {
        EmulatorDetection = EmulatorDetection.EmulatorOrProduction,
        Credential = GoogleCredential.FromJson(serviceAccountKeyJson)
    }.BuildAsync();

// Use the client as you'd normally do, to create a subscription in this example.
    SubscriptionName subscriptionName = new SubscriptionName("bachelorshenanigans", "testsub");
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

    Console.WriteLine("Server started. Setting up subscription...");
// Use the client as you'd normally do, to listen for messages in this example.
    await subscriber.StartAsync((msg, cancellationToken) =>
    {
        Console.WriteLine("Pubsub trigger");
        var messageContents = msg.Data.ToStringUtf8();
        Console.WriteLine(messageContents);
        var events = JsonSerializer.Deserialize<List<Event>>(messageContents, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        Console.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
        try
        {
            using (var connection =
                   new NpgsqlConnection(
                       "Server=34.159.144.157;Port=5432;Database=postgres;User Id=postgres;Password=postgres"))
            {
                connection.Open();
                Console.WriteLine("Inserting new event");
                events.ForEach(e =>
                {
                    const string cmd =
                        "INSERT INTO public.event(title, url, location, description) values (@title, @url, @location, @description)";
                    var parameters = new
                    {
                        @title = e.Title, @url = e.Url, @description = e.Description,
                        @location = JsonSerializer.Serialize(e.Location)
                    };

                    connection.Execute(cmd, parameters);
                });
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        // In this example we stop the subscriber when the message is received.
        // You may leave the subscriber running, and it will continue to received published messages
        // if any.
        // This is non-blocking, and the returned Task may be awaited.
        //subscriber.StopAsync(TimeSpan.FromSeconds(15));
        // Return Reply.Ack to indicate this message has been handled.
        return Task.FromResult(SubscriberClient.Reply.Ack);
    });
});
//thread.IsBackground = false;
thread.Start();
//


// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();