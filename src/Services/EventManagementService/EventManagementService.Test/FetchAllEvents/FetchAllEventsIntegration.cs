using EventManagementService.Application.FetchAllEvents;
using EventManagementService.Application.FetchAllEvents.Exceptions;
using EventManagementService.Application.FetchAllEvents.Model;
using EventManagementService.Application.FetchAllEvents.Repository;
using EventManagementService.Domain.Models;
using EventManagementService.Domain.Models.Events;
using EventManagementService.Infrastructure;
using EventManagementService.Test.Shared;
using EventManagementService.Test.Shared.Builders;
using Microsoft.Extensions.Logging;
using Moq;

namespace EventManagementService.Test.FetchAllEvents;

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
        var eventRepositoryMock = new Mock<ISqlAllEvents>();
        var loggerMock = new Mock<ILogger<FetchAllEventsHandler>>();
        var userRepositoryMock = new Mock<IUserRepository>();

        var testEvents = new List<Event>
            {dataBuilder.NewTestEvent(e => e.Title = "E1"), dataBuilder.NewTestEvent(e => e.Title = "E2")};
        var filters = new Filters();

        dataBuilder.InsertEvents(testEvents);
        for (var i = 0; i < dataBuilder.EventSet.Count; i++)
        {
            testEvents[i].Id = dataBuilder.EventSet[i].Id;
        }
        
        eventRepositoryMock.Setup(x => x.GetAllEvents(filters)).ReturnsAsync(testEvents);
        userRepositoryMock.Setup(x => x.GetUsersAsync(It.IsAny<IReadOnlyCollection<string>>()))
            .ReturnsAsync(new List<User>(){testEvents[0].Host});

        var request = new FetchAllEventsRequest(filters);
        var handler =
            new FetchAllEventsHandler(eventRepositoryMock.Object, loggerMock.Object, userRepositoryMock.Object);
        
        // Act
        var events = await handler.Handle(request, new CancellationToken());
        
        // Assert
        Assert.That(events.Count, Is.EqualTo(2));
        Assert.That(events.ToList()[0].Title, Is.EqualTo("E1"));
        Assert.That(events.ToList()[1].Title, Is.EqualTo("E2"));
    }
}
