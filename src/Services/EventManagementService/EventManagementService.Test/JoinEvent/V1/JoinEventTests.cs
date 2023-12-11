using EventManagementService.Application.V1.JoinEvent;
using EventManagementService.Application.V1.JoinEvent.Exceptions;
using EventManagementService.Application.V1.JoinEvent.Repositories;
using EventManagementService.Domain.Models;
using EventManagementService.Domain.Models.Events;
using EventManagementService.Infrastructure;
using EventManagementService.Infrastructure.AppSettings;
using EventManagementService.Infrastructure.EventBus;
using EventManagementService.Infrastructure.Notifications;
using EventManagementService.Test.Shared;
using EventManagementService.Test.Shared.Builders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace EventManagementService.Test.JoinEvent.V1;

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
        // Arrange
        var dataBuilder = new DataBuilder(_connectionStringManager);
        var nonExistingUserId = "Oq8tmUrDV6SeEpWf1olCJNJ1JW93";
        var existingEvent = dataBuilder.NewTestEvent();

        var invitationRepositoryMock = new Mock<IInvitationRepository>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var eventRepositoryMock = new Mock<IEventRepository>();
        var eventBusMock = new Mock<IEventBus>();
        var notifierMock = new Mock<INotifier>();
        var pubsubConfig = new PubSub
        {
            Topics = new[]
            {
                new Topic()
                {
                    ProjectId = "test",
                    TopicId = "test",
                    SubscriptionNames = new[] { "test" }
                },
                new Topic()
                {
                    ProjectId = "test",
                    TopicId = "test",
                    SubscriptionNames = new[] { "test" }
                }
            },
        };
        var loggerMock = new Mock<ILogger<JoinEventHandler>>();

        userRepositoryMock.Setup(x => x.UserExistsAsync(nonExistingUserId)).ReturnsAsync(false);
        eventRepositoryMock.Setup(x => x.GetByIdAsync(existingEvent.Id)).ReturnsAsync(existingEvent);

        var handler = new JoinEventHandler(loggerMock.Object, eventRepositoryMock.Object,
            invitationRepositoryMock.Object, userRepositoryMock.Object, eventBusMock.Object,
            Options.Create(pubsubConfig), notifierMock.Object);
        var request = new JoinEventRequest(nonExistingUserId, existingEvent.Id);

        Assert.ThrowsAsync<UserNotFoundException>(() => handler.Handle(request, new CancellationToken()));
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
        var eventBusMock = new Mock<IEventBus>();
        var notifierMock = new Mock<INotifier>();
        var pubsubConfig = new PubSub
        {
            Topics = new[]
            {
                new Topic()
                {
                    ProjectId = "test",
                    TopicId = "test",
                    SubscriptionNames = new []{"test"}
                },
                new Topic()
                {
                    ProjectId = "test",
                    TopicId = "test",
                    SubscriptionNames = new []{"test"}
                }
            },
        };

        userRepositoryMock.Setup(x => x.UserExistsAsync(existingUserId)).ReturnsAsync(true);
        eventRepositoryMock.Setup(x => x.GetByIdAsync(nonExistingEventId)).ReturnsAsync((Event?)null);

        var handler = new JoinEventHandler(loggerMock.Object, eventRepositoryMock.Object,
            invitationRepositoryMock.Object, userRepositoryMock.Object, eventBusMock.Object,
            Options.Create(pubsubConfig), notifierMock.Object);
        var request = new JoinEventRequest(existingUserId, nonExistingEventId);

        Assert.ThrowsAsync<EventNotFoundException>(() => handler.Handle(request, new CancellationToken()));
    }

    [Test]
    public void JoinEvent_UserHasAlreadyJoinedTheEvent_ThrowsAlreadyJoinedException()
    {
        // Arrange
        var databuilder = new DataBuilder(_connectionStringManager);
        var existingUserId = "Oq8tmUrDV6SeEpWf1olCJNJ1JW93";
        var existingEvent =
            databuilder.NewTestEvent(e => e.Attendees = new List<User> { new User() { UserId = existingUserId } });

        var invitationRepositoryMock = new Mock<IInvitationRepository>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var eventRepositoryMock = new Mock<IEventRepository>();
        var loggerMock = new Mock<ILogger<JoinEventHandler>>();
        var eventBusMock = new Mock<IEventBus>();
        var notifierMock = new Mock<INotifier>();
        var pubsubConfig = new PubSub
        {
            Topics = new[]
            {
                new Topic()
                {
                    ProjectId = "test",
                    TopicId = "test",
                    SubscriptionNames = new[] { "test" }
                },
                new Topic()
                {
                    ProjectId = "test",
                    TopicId = "test",
                    SubscriptionNames = new[] { "test" }
                }
            },
        };

        userRepositoryMock.Setup(x => x.UserExistsAsync(existingUserId)).ReturnsAsync(true);
        eventRepositoryMock.Setup(x => x.GetByIdAsync(existingEvent.Id)).ReturnsAsync(existingEvent);

        var handler = new JoinEventHandler(loggerMock.Object, eventRepositoryMock.Object,
            invitationRepositoryMock.Object, userRepositoryMock.Object, eventBusMock.Object,
            Options.Create(pubsubConfig), notifierMock.Object);
        var request = new JoinEventRequest(existingUserId, existingEvent.Id);

        Assert.ThrowsAsync<AlreadyJoinedException>(() => handler.Handle(request, new CancellationToken()));
    }

    [Test]
    public void JoinEvent_MaxNumberOfAttendeesHasAlreadyBeenReached_ThrowsMaximumAttendeesReachedException()
    {
        // Arrange
        var databuilder = new DataBuilder(_connectionStringManager);
        var existingUserId = "Oq8tmUrDV6SeEpWf1olCJNJ1JW93";
        var existingEvent = databuilder.NewTestEvent(e =>
        {
            e.Attendees = new List<User>
            {
                new User { UserId = Guid.NewGuid().ToString() },
                new User { UserId = Guid.NewGuid().ToString() }
            };

            e.MaxNumberOfAttendees = 2;
        });

        var invitationRepositoryMock = new Mock<IInvitationRepository>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var eventRepositoryMock = new Mock<IEventRepository>();
        var loggerMock = new Mock<ILogger<JoinEventHandler>>();
        var eventBusMock = new Mock<IEventBus>();
        var notifierMock = new Mock<INotifier>();
        var pubsubConfig = new PubSub
        {
            Topics = new[]
            {
                new Topic()
                {
                    ProjectId = "test",
                    TopicId = "test",
                    SubscriptionNames = new []{"test"}
                },
                new Topic()
                {
                    ProjectId = "test",
                    TopicId = "test",
                    SubscriptionNames = new []{"test"}
                }
            },
        };

        userRepositoryMock.Setup(x => x.UserExistsAsync(existingUserId)).ReturnsAsync(true);
        eventRepositoryMock.Setup(x => x.GetByIdAsync(existingEvent.Id)).ReturnsAsync(existingEvent);

        var handler = new JoinEventHandler(loggerMock.Object, eventRepositoryMock.Object,
            invitationRepositoryMock.Object, userRepositoryMock.Object, eventBusMock.Object,
            Options.Create(pubsubConfig), notifierMock.Object);
        var request = new JoinEventRequest(existingUserId, existingEvent.Id);

        Assert.ThrowsAsync<MaximumAttendeesReachedException>(() => handler.Handle(request, new CancellationToken()));
    }

    [Test]
    public void JoinEvent_UserIsHostOfEvent_ThrowsUserIsAlreadyHostOfEventException()
    {
        // Arrange
        var databuilder = new DataBuilder(_connectionStringManager);
        var existingUserId = "Oq8tmUrDV6SeEpWf1olCJNJ1JW93";
        var existingEvent = databuilder.NewTestEvent(e =>
        {
            e.Attendees = new List<User>
            {
                new User { UserId = Guid.NewGuid().ToString() }
            };

            e.MaxNumberOfAttendees = 2;
            e.Host = new User { UserId = existingUserId };
        });

        var invitationRepositoryMock = new Mock<IInvitationRepository>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var eventRepositoryMock = new Mock<IEventRepository>();
        var loggerMock = new Mock<ILogger<JoinEventHandler>>();
        var eventBusMock = new Mock<IEventBus>();
        var notifierMock = new Mock<INotifier>();
        var pubsubConfig = new PubSub
        {
            Topics = new[]
            {
                new Topic()
                {
                    ProjectId = "test",
                    TopicId = "test",
                    SubscriptionNames = new[] { "test" }
                },
                new Topic()
                {
                    ProjectId = "test",
                    TopicId = "test",
                    SubscriptionNames = new[] { "test" }
                }
            },
        };

        userRepositoryMock.Setup(x => x.UserExistsAsync(existingUserId)).ReturnsAsync(true);
        eventRepositoryMock.Setup(x => x.GetByIdAsync(existingEvent.Id)).ReturnsAsync(existingEvent);

        var handler = new JoinEventHandler(loggerMock.Object, eventRepositoryMock.Object,
            invitationRepositoryMock.Object, userRepositoryMock.Object, eventBusMock.Object,
            Options.Create(pubsubConfig), notifierMock.Object);
        var request = new JoinEventRequest(existingUserId, existingEvent.Id);

        Assert.ThrowsAsync<UserIsAlreadyHostOfEventException>(() => handler.Handle(request, new CancellationToken()));
    }
}