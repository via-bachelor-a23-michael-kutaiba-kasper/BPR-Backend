using EventManagementService.Application.FetchEventById;
using EventManagementService.Application.FetchEventById.Repositories;
using EventManagementService.Domain.Models;
using EventManagementService.Infrastructure;
using EventManagementService.Test.Shared;
using EventManagementService.Test.Shared.Builders;
using Microsoft.Extensions.Logging;
using Moq;

namespace EventManagementService.Test.FetchEventById;

[TestFixture]
public class FetchEventByIdIntegration
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
    public async Task FetchEventById_EventExists_ReturnsEvent()
    {
        // Arrange
        var dataBuilder = new DataBuilder(_connectionStringManager);
        var testEvent = dataBuilder.NewTestEvent((e) => e.Title = "Test Event");
        dataBuilder.InsertEvents(new[] { testEvent });
        var hostId = testEvent.Host.UserId;
        testEvent.Id = dataBuilder.EventSet[0].Id;
        var testUser = new User
        {
            UserId = hostId,
            DisplayName = "Test User"
        };
        
        var loggerMock = new Mock<ILogger<FetchEventByIdHandler>>();
        var loggerMock2 = new Mock<ILogger<EventRepository>>();
        var eventRepository = new EventRepository(loggerMock2.Object, _connectionStringManager);
        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(x => x.GetUserById(hostId)).ReturnsAsync(testUser);


        var testRequest = new FetchEventByIdRequest(testEvent.Id);
        var handler = new FetchEventByIdHandler(loggerMock.Object, eventRepository, userRepositoryMock.Object);

        // Act 
        var fetchedEvent = await handler.Handle(testRequest, new CancellationToken());
        
        // Assert
        Assert.That(fetchedEvent, Is.Not.Null);
        Assert.That(fetchedEvent.Host.DisplayName, Is.EqualTo("Test User"));
        Assert.That(fetchedEvent.Title, Is.EqualTo("Test Event"));
    }
}