using System.Text.Json;
using System.Text.Json.Serialization;
using EventManagementService.Application.V1.ProcessExternalEvents;
using EventManagementService.Application.V1.ProcessExternalEvents.Exceptions;
using EventManagementService.Application.V1.ProcessExternalEvents.Repository;
using EventManagementService.Domain.Models.Events;
using EventManagementService.Domain.Models.Google;
using EventManagementService.Infrastructure;
using EventManagementService.Test.Shared;
using Microsoft.Extensions.Logging;
using Moq;

namespace EventManagementService.Test.ProcessExternalEvents.V1.IntegrationTests;

[TestFixture]
public class ProcessExternalEventsTests
{
    private readonly TestDataContext _context = new();
    private readonly ConnectionStringManager _connectionStringManager = new();

    [SetUp]
    public async Task Setup()
    {
        _context.ConnectionString = _connectionStringManager.GetConnectionString();
        await _context.Clean();
    }

    [TearDown]
    public async Task TearDown()
    {
        _context.ConnectionString = _connectionStringManager.GetConnectionString();
        await _context.Clean();
    }

    [Test]
    public async Task ProcessExternalEvents_Handle_ThrowNoExceptions()
    {
        // Arrange
        var externalEventsPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "ProcessExternalEvents",
            "Fakes",
            "ExternalEventsResponse.json"
        );
        
        var eventsPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "ProcessExternalEvents",
            "Fakes",
            "ProcessedExternalEvents.json"
        );
        
        var geoPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "ProcessExternalEvents",
            "Fakes",
            "GoogleGeoCodeResponse.json"
        );

        var eventsFile = await File.ReadAllTextAsync(eventsPath);
        var events = JsonSerializer.Deserialize<List<Event>>(eventsFile, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });
        
        var externalEventsFile = await File.ReadAllTextAsync(externalEventsPath);
        var externalEvents = JsonSerializer.Deserialize<List<Event>>(externalEventsFile, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });

        var geoFile = await File.ReadAllTextAsync(geoPath);
        var geoLocations = JsonSerializer.Deserialize<GoogleGeoLocation>(geoFile, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });

        var loggerMock = new Mock<ILogger<ProcessExternalEventsHandler>>();
        var pubSubRepoMock = new Mock<IPubSubExternalEvents>();
        var sqlRepoMock = new Mock<ISqlExternalEvents>();
        var geoCodingRepoMock = new Mock<IGeoCoding>();

        pubSubRepoMock.Setup(x => x.FetchEvents(new CancellationToken())).ReturnsAsync(externalEvents!);
        sqlRepoMock.Setup(x => x.BulkUpsertEvents(events!));
        geoCodingRepoMock.Setup(x => x.FetchGeoLocationForAddress("Fussingvej 8, 8700 Horsens, Denmark"))!
            .ReturnsAsync(geoLocations);

        var handler = new ProcessExternalEventsHandler(
            geoCodingRepoMock.Object,
            pubSubRepoMock.Object,
            sqlRepoMock.Object,
            loggerMock.Object
        );
        // Act
        var act = async () => await handler.Handle(
            new ProcessExternalEventsRequest(),
            new CancellationToken()
        );

        // Assert
        Assert.DoesNotThrowAsync(() => act.Invoke());
    }
    
     [Test]
    public async Task ProcessExternalEvents_HandleException_ThrowCreateNewEventsException()
    {
        // Arrange
        var externalEventsPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "ProcessExternalEvents",
            "Fakes",
            "ExternalEventsResponse.json"
        );
        
        var eventsPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "ProcessExternalEvents",
            "Fakes",
            "ProcessedExternalEvents.json"
        );
        
        var geoPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "ProcessExternalEvents",
            "Fakes",
            "GoogleGeoCodeResponse.json"
        );

        var eventsFile = await File.ReadAllTextAsync(eventsPath);
        var events = JsonSerializer.Deserialize<List<Event>>(eventsFile, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });
        
        var externalEventsFile = await File.ReadAllTextAsync(externalEventsPath);
        var externalEvents = JsonSerializer.Deserialize<List<Event>>(externalEventsFile, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });

        var geoFile = await File.ReadAllTextAsync(geoPath);
        var geoLocations = JsonSerializer.Deserialize<GoogleGeoLocation>(geoFile, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });

        var loggerMock = new Mock<ILogger<ProcessExternalEventsHandler>>();
        var pubSubRepoMock = new Mock<IPubSubExternalEvents>();
        var sqlRepoMock = new Mock<ISqlExternalEvents>();
        var geoCodingRepoMock = new Mock<IGeoCoding>();

        pubSubRepoMock.Setup(x => x.FetchEvents(new CancellationToken())).ReturnsAsync(externalEvents!);
        sqlRepoMock.Setup(x => x.BulkUpsertEvents(It.IsAny<List<Event>>())).ThrowsAsync(new UpsertEventsException());
        geoCodingRepoMock.Setup(x => x.FetchGeoLocationForAddress("Fussingvej 8, 8700 Horsens, Denmark"))!
            .ReturnsAsync(geoLocations);

        var handler = new ProcessExternalEventsHandler(
            geoCodingRepoMock.Object,
            pubSubRepoMock.Object,
            sqlRepoMock.Object,
            loggerMock.Object
        );
        // Act
        var act = async () => await handler.Handle(
            new ProcessExternalEventsRequest(),
            new CancellationToken()
        );

        // Assert
        Assert.ThrowsAsync<CreateNewEventsException>(() => act.Invoke());
    }
}