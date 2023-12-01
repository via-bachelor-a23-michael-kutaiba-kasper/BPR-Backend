using EventManagementService.Application.V1.FetchEventById;
using EventManagementService.Application.V1.FetchEventById.Exceptions;
using EventManagementService.Application.V1.FetchEventById.Repositories;
using EventManagementService.Domain.Models.Events;
using EventManagementService.Infrastructure;
using EventManagementService.Test.Shared;
using Microsoft.Extensions.Logging;
using Moq;

namespace EventManagementService.Test.FetchEventById.V1;

[TestFixture]
public class FetchEventByIdTest
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
    public void FetchEventById_EventDoesNotExist_ThrowsEventNotFoundException()
    {
        // Arrange
        var nonExistingEventId = 1;
        var eventRepositoryMock = new Mock<IEventRepository>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var loggerMock = new Mock<ILogger<FetchEventByIdHandler>>();

        eventRepositoryMock.Setup(x => x.GetEventByIdAsync(nonExistingEventId)).ReturnsAsync((Event?)null);

        var testRequest = new FetchEventByIdRequest(nonExistingEventId);
        var handler = new FetchEventByIdHandler(loggerMock.Object, eventRepositoryMock.Object, userRepositoryMock.Object);

        // Act & Assert
        Assert.ThrowsAsync<EventNotFoundException>(() => handler.Handle(testRequest, new CancellationToken()));
    }
}