using EventManagementService.Application.V1.FetchAllEvents;
using EventManagementService.Application.V1.FetchAllEvents.Model;
using EventManagementService.Application.V1.FetchAllEvents.Repository;
using EventManagementService.Domain.Models;
using EventManagementService.Domain.Models.Events;
using EventManagementService.Infrastructure;
using EventManagementService.Test.Shared;
using EventManagementService.Test.Shared.Builders;
using Microsoft.Extensions.Logging;
using Moq;

namespace EventManagementService.Test.FetchAllEvents.V1;

[TestFixture]
public class FetchAllEventsIntegration
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
    public async Task FetchAllEvents_NoFilters_ReturnsAllEvents()
    {
        // Arrange 
        var dataBuilder = new DataBuilder(_connectionStringManager);
        var loggerMock = new Mock<ILogger<FetchAllEventsHandler>>();
        var loggerMock2 = new Mock<ILogger<SqlAllEvents>>();
        var userRepositoryMock = new Mock<IUserRepository>();
        
        ISqlAllEvents eventRepository = new SqlAllEvents(_connectionStringManager, loggerMock2.Object);

        var testEvents = new List<Event>
            {dataBuilder.NewTestEvent(e => e.Title = "E1"), dataBuilder.NewTestEvent(e => e.Title = "E2")};
        var filters = new Filters();

        dataBuilder.InsertEvents(testEvents);
        for (var i = 0; i < dataBuilder.EventSet.Count; i++)
        {
            testEvents[i].Id = dataBuilder.EventSet[i].Id;
        }
        
        userRepositoryMock.Setup(x => x.GetUsersAsync(It.IsAny<IReadOnlyCollection<string>>()))
            .ReturnsAsync(new List<User>() {testEvents[0].Host});

        var request = new FetchAllEventsRequest(filters);
        var handler =
            new FetchAllEventsHandler(eventRepository, loggerMock.Object, userRepositoryMock.Object);

        // Act
        var events = await handler.Handle(request, new CancellationToken());

        // Assert
        Assert.That(events.Count, Is.EqualTo(2));
        Assert.That(events.ToList()[0].Title, Is.EqualTo("E1"));
        Assert.That(events.ToList()[1].Title, Is.EqualTo("E2"));
    }

    [Test]
    public async Task FetchAllEvents_FromFilterIsApplied_ReturnsEventsFilteredByDate()
    {
        // Arrange 
        var dataBuilder = new DataBuilder(_connectionStringManager);
        var loggerMock = new Mock<ILogger<FetchAllEventsHandler>>();
        var loggerMock2 = new Mock<ILogger<SqlAllEvents>>();
        var userRepositoryMock = new Mock<IUserRepository>();
        
        ISqlAllEvents eventRepository = new SqlAllEvents(_connectionStringManager, loggerMock2.Object);

        var user1Id = Guid.NewGuid().ToString();
        var user2Id = Guid.NewGuid().ToString();
        var testEvents = new List<Event>
        {
            dataBuilder.NewTestEvent(e =>
            {
                e.Title = "E1";
                e.Host.UserId = user1Id;
                e.StartDate= DateTimeOffset.UtcNow.AddMinutes(-20);
                e.EndDate = DateTimeOffset.UtcNow.AddMinutes(10);
            }),
            dataBuilder.NewTestEvent(e =>
            {
                e.Title = "E2";
                e.Host.UserId = user2Id;
                e.StartDate= DateTimeOffset.UtcNow.AddMinutes(-20);
                e.EndDate = DateTimeOffset.UtcNow.AddMinutes(-10);
            })
        };
        var filters = new Filters{From = DateTimeOffset.UtcNow};

        dataBuilder.InsertEvents(testEvents);
        for (var i = 0; i < dataBuilder.EventSet.Count; i++)
        {
            testEvents[i].Id = dataBuilder.EventSet[i].Id;
        }

        userRepositoryMock.Setup(x => x.GetUsersAsync(It.IsAny<IReadOnlyCollection<string>>()))
            .ReturnsAsync(new List<User>() {testEvents[0].Host, testEvents[1].Host});

        var request = new FetchAllEventsRequest(filters);
        var handler =
            new FetchAllEventsHandler(eventRepository, loggerMock.Object, userRepositoryMock.Object);

        // Act
        var events = await handler.Handle(request, new CancellationToken());

        // Assert
        Assert.That(events.Count, Is.EqualTo(1));
        Assert.That(events.ToList()[0].Title, Is.EqualTo("E1"));
    }
    
    [Test]
    public async Task FetchAllEvents_HostIdFilterIsApplied_ReturnsEventsFilteredByHost()
    {
        // Arrange 
        var dataBuilder = new DataBuilder(_connectionStringManager);
        var loggerMock = new Mock<ILogger<FetchAllEventsHandler>>();
        var loggerMock2 = new Mock<ILogger<SqlAllEvents>>();
        var userRepositoryMock = new Mock<IUserRepository>();
        
        ISqlAllEvents eventRepository = new SqlAllEvents(_connectionStringManager, loggerMock2.Object);

        var user1Id = Guid.NewGuid().ToString();
        var user2Id = Guid.NewGuid().ToString();
        var testEvents = new List<Event>
        {
            dataBuilder.NewTestEvent(e =>
            {
                e.Title = "E1";
                e.Host.UserId = user1Id;
                e.StartDate= DateTimeOffset.UtcNow.AddMinutes(-20);
                e.EndDate = DateTimeOffset.UtcNow.AddMinutes(10);
            }),
            dataBuilder.NewTestEvent(e =>
            {
                e.Title = "E2";
                e.Host.UserId = user2Id;
                e.StartDate= DateTimeOffset.UtcNow.AddMinutes(-20);
                e.EndDate = DateTimeOffset.UtcNow.AddMinutes(-10);
            })
        };
        var filters = new Filters{HostId = user2Id};

        dataBuilder.InsertEvents(testEvents);
        for (var i = 0; i < dataBuilder.EventSet.Count; i++)
        {
            testEvents[i].Id = dataBuilder.EventSet[i].Id;
        }

        userRepositoryMock.Setup(x => x.GetUsersAsync(It.IsAny<IReadOnlyCollection<string>>()))
            .ReturnsAsync(new List<User>() {testEvents[0].Host, testEvents[1].Host});

        var request = new FetchAllEventsRequest(filters);
        var handler =
            new FetchAllEventsHandler(eventRepository, loggerMock.Object, userRepositoryMock.Object);

        // Act
        var events = await handler.Handle(request, new CancellationToken());

        // Assert
        Assert.That(events.Count, Is.EqualTo(1));
        Assert.That(events.ToList()[0].Title, Is.EqualTo("E2"));
    }
}