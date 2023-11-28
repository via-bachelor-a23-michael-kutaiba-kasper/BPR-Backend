using EventManagementService.Application.JoinEvent;
using EventManagementService.Application.JoinEvent.Exceptions;
using EventManagementService.Application.JoinEvent.Repositories;
using EventManagementService.Domain.Models.Events;
using EventManagementService.Infrastructure;
using EventManagementService.Test.JoinEvent.Utils;
using EventManagementService.Test.Shared;
using Microsoft.Extensions.Logging;
using Moq;

namespace EventManagementService.Test.JoinEvent;

[TestFixture]
public class JoinEventTests
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
    public void JoinEvent_UserDoesExist_ThrowsUserNotFoundException()
    {
        /*// Arrange
        var dataBuilder = new DataBuilder(_connectionStringManager);
        var nonExistingUserId = "Oq8tmUrDV6SeEpWf1olCJNJ1JW93";
        var existingEvent = dataBuilder.NewTestEvent();

        var invitationRepositoryMock = new Mock<IInvitationRepository>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var eventRepositoryMock = new Mock<IEventRepository>();
        var loggerMock = new Mock<ILogger<JoinEventHandler>>();

        userRepositoryMock.Setup(x => x.UserExistsAsync(nonExistingUserId)).ReturnsAsync(false);
        eventRepositoryMock.Setup(x => x.GetByIdAsync(existingEvent.Id)).ReturnsAsync(existingEvent);

        var handler = new JoinEventHandler(loggerMock.Object, eventRepositoryMock.Object,
            invitationRepositoryMock.Object, userRepositoryMock.Object);
        var request = new JoinEventRequest(nonExistingUserId, existingEvent.Id);

        Assert.ThrowsAsync<UserNotFoundException>(() => handler.Handle(request, new CancellationToken()));*/
    }

    [Test]
    public void JoinEvent_EventDoesNotExist_ThrowsEventNotFoundException()
    {
        // Arrange
        var existingUserId = "Oq8tmUrDV6SeEpWf1olCJNJ1JW93";
        var nonExistingEventId = 1;

        var invitationRepositoryMock = new Mock<IInvitationRepository>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var eventRepositoryMock = new Mock<IEventRepository>();
        var loggerMock = new Mock<ILogger<JoinEventHandler>>();

        userRepositoryMock.Setup(x => x.UserExistsAsync(existingUserId)).ReturnsAsync(true);
        eventRepositoryMock.Setup(x => x.GetByIdAsync(nonExistingEventId)).ReturnsAsync((Event?)null);

        var handler = new JoinEventHandler(loggerMock.Object, eventRepositoryMock.Object,
            invitationRepositoryMock.Object, userRepositoryMock.Object);
        var request = new JoinEventRequest(existingUserId, nonExistingEventId);

        Assert.ThrowsAsync<EventNotFoundException>(() => handler.Handle(request, new CancellationToken()));
    }

    [Test]
    public void JoinEvent_UserHasAlreadyJoinedTheEvent_ThrowsAlreadyJoinedException()
    {
        /*// Arrange
        var databuilder = new DataBuilder(_connectionStringManager);
        var existingUserId = "Oq8tmUrDV6SeEpWf1olCJNJ1JW93";
        var existingEvent = databuilder.NewTestEvent(e => e.Attendees = new List<string> { existingUserId });

        var invitationRepositoryMock = new Mock<IInvitationRepository>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var eventRepositoryMock = new Mock<IEventRepository>();
        var loggerMock = new Mock<ILogger<JoinEventHandler>>();

        userRepositoryMock.Setup(x => x.UserExistsAsync(existingUserId)).ReturnsAsync(true);
        eventRepositoryMock.Setup(x => x.GetByIdAsync(existingEvent.Id)).ReturnsAsync(existingEvent);

        var handler = new JoinEventHandler(loggerMock.Object, eventRepositoryMock.Object,
            invitationRepositoryMock.Object, userRepositoryMock.Object);
        var request = new JoinEventRequest(existingUserId, existingEvent.Id);

        Assert.ThrowsAsync<AlreadyJoinedException>(() => handler.Handle(request, new CancellationToken()));*/
    }
}