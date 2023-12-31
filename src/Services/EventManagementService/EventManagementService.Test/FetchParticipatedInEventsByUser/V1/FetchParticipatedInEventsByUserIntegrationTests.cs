using EventManagementService.Application.V1.FetchFinishedParticipatedInEventsByUser;
using EventManagementService.Application.V1.FetchFinishedParticipatedInEventsByUser.Exceptions;
using EventManagementService.Application.V1.FetchFinishedParticipatedInEventsByUser.Repository;
using EventManagementService.Domain.Models;
using EventManagementService.Domain.Models.Events;
using EventManagementService.Infrastructure;
using EventManagementService.Infrastructure.Util;
using EventManagementService.Test.Shared;
using EventManagementService.Test.Shared.Builders;
using Microsoft.Extensions.Logging;
using Moq;

namespace EventManagementService.Test.FetchParticipatedInEventsByUser.V1;

[TestFixture]
public class FetchParticipatedInEventsByUserIntegrationTests
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
    public async Task FetchFinishedJoinedEvents_ByUserId_ThrowNoExceptions()
    {
        // Arrange
        const string userId = "Oq8tmUrDV6SeEpWf1olCJNJ1JW93";
        var dataBuilder = new DataBuilder(_connectionStringManager);
        var sqlLogger = new Mock<ILogger<SqlEvent>>();
        var firebaseLogger = new Mock<ILogger<FirebaseUser>>();
        var firebaseMock = new Mock<IFirebaseUser>();
        var handlerLogger = new Mock<ILogger<FetchParticipatedEventsByUserHandler>>();
        var testEvents = new List<Event>
        {
            dataBuilder.NewTestEvent(e =>
            {
                e.Title = "Test1";
                e.Attendees = new[]
                {
                    new User
                    {
                        UserId = userId,
                        CreationDate = DateTimeOffset.UtcNow.ToUniversalTime()
                    }
                };
                e.EndDate = new DateTimeOffset(2022, 1, 1, 12, 0, 0, TimeSpan.Zero);
            }),
            dataBuilder.NewTestEvent(e => e.Title = "Test2")
        };
        
        dataBuilder.InsertEvents(testEvents);
        for (var i = 0; i < dataBuilder.EventSet.Count; i++)
        {
            testEvents[i].Id = dataBuilder.EventSet[i].Id;
        }
        firebaseMock.Setup(x => x.GetUsers(It.IsAny<IReadOnlyCollection<string>>()))
            .ReturnsAsync(new List<User>() {testEvents[0].Host});

        var request = new FetchParticipatedInEventsByUserRequest(userId);
        var handler = new FetchParticipatedEventsByUserHandler
        (
            new SqlEvent(sqlLogger.Object, _connectionStringManager),
            handlerLogger.Object,
            firebaseMock.Object
        );

        // Act
        var act = async () => await handler.Handle(request, new CancellationToken());

        // Assert
        Assert.DoesNotThrowAsync(() => act.Invoke());
    }


    [Test]
    public async Task FetchFinishedJoinedEvents_ByUserId_ReturnsCorrectData()
    {
        // Arrange
        const string userId = "Oq8tmUrDV6SeEpWf1olCJNJ1JW93";
        var dataBuilder = new DataBuilder(_connectionStringManager);
        var sqlLogger = new Mock<ILogger<SqlEvent>>();
        var firebaseLogger = new Mock<ILogger<FirebaseUser>>();
        var handlerLogger = new Mock<ILogger<FetchParticipatedEventsByUserHandler>>();
        var firebaseMock = new Mock<IFirebaseUser>();
        var testEvents = new List<Event>
        {
            dataBuilder.NewTestEvent(e =>
            {
                e.Title = "Test1"; 
                e.Attendees = new[]
                {
                    new User
                    {
                        UserId = userId,
                        CreationDate = DateTimeOffset.UtcNow.ToUniversalTime()
                    },
                    new User
                    {
                        UserId = "test",
                        CreationDate = DateTimeOffset.UtcNow.ToUniversalTime()
                    }
                };
            }),
            dataBuilder.NewTestEvent(e =>
            {
                e.Title = "Test2"; 
                e.Attendees = new[]
                {
                    new User
                    {
                        UserId = userId,
                        CreationDate = DateTimeOffset.UtcNow.ToUniversalTime()
                    },
                    new User
                    {
                        UserId = "test",
                        CreationDate = DateTimeOffset.UtcNow.ToUniversalTime()
                    }
                };
            })
        };

        testEvents[0].EndDate = new DateTimeOffset(2022, 1, 1, 12, 0, 0, TimeSpan.Zero);
        dataBuilder.InsertEvents(testEvents);
        for (var i = 0; i < dataBuilder.EventSet.Count; i++)
        {
            testEvents[i].Id = dataBuilder.EventSet[i].Id;
        }
        firebaseMock.Setup(x => x.GetUsers(It.IsAny<IReadOnlyCollection<string>>()))
            .ReturnsAsync(new List<User>() {testEvents[0].Host});

        var request = new FetchParticipatedInEventsByUserRequest(userId);
        var handler = new FetchParticipatedEventsByUserHandler
        (
            new SqlEvent(sqlLogger.Object, _connectionStringManager),
            handlerLogger.Object,
            firebaseMock.Object
        );

        // Act
        var events = await handler.Handle(request, new CancellationToken());

        // Assert
        Assert.That(events.Count(), Is.EqualTo(1));
        foreach (var ev in events)
        {
            Assert.That
            (
                ev.EndDate.ToUniversalTime().Truncate(TimeSpan.FromSeconds(1)),
                Is.LessThan(DateTimeOffset.UtcNow.Truncate(TimeSpan.FromSeconds(1)))
            );
        }
    }
    
    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public async Task FetchFinishedJoinedEvents_ByUserInvalidUserId_ThrowsInvalidUserIdException(string userId)
    {
        // Arrange
        //const string userId = "Oq8tmUrDV6SeEpWf1olCJNJ1JW93";
        var firebaseLogger = new Mock<ILogger<FirebaseUser>>();
        var dataBuilder = new DataBuilder(_connectionStringManager);
        var sqlLogger = new Mock<ILogger<SqlEvent>>();
        var handlerLogger = new Mock<ILogger<FetchParticipatedEventsByUserHandler>>();
        var testEvents = new List<Event>
        {
            dataBuilder.NewTestEvent(e =>
            {
                e.Title = "Test1"; 
                e.Attendees = new[]
                {
                    new User()
                    {
                        UserId = userId,
                        CreationDate = DateTimeOffset.UtcNow.ToUniversalTime()
                    },
                    new User()
                    {
                        UserId = "test",
                        CreationDate = DateTimeOffset.UtcNow.ToUniversalTime()
                    }
                };
            }),
            dataBuilder.NewTestEvent(e =>
            {
                e.Title = "Test2"; 
                e.Attendees = new[]
                {
                    new User()
                    {
                        UserId = userId,
                        CreationDate = DateTimeOffset.UtcNow.ToUniversalTime()
                    },
                    new User()
                    {
                        UserId = "test",
                        CreationDate = DateTimeOffset.UtcNow.ToUniversalTime()
                    }
                };
            })
        };

        testEvents[0].EndDate = new DateTimeOffset(2022, 1, 1, 12, 0, 0, TimeSpan.Zero);
        dataBuilder.InsertEvents(testEvents);
        for (var i = 0; i < dataBuilder.EventSet.Count; i++)
        {
            testEvents[i].Id = dataBuilder.EventSet[i].Id;
        }
        

        var request = new FetchParticipatedInEventsByUserRequest(userId);
        var handler = new FetchParticipatedEventsByUserHandler
        (
            new SqlEvent(sqlLogger.Object, _connectionStringManager),
            handlerLogger.Object,
            new FirebaseUser(firebaseLogger.Object)
        );

        // Act
        var act = async () => await handler.Handle(request, new CancellationToken());
        var exception = Assert.ThrowsAsync<FetchFinishedParticipatedInEventsByUserException>(() => act.Invoke());

        // Assert
        Assert.That(exception!.InnerException!, Is.TypeOf<InvalidUserIdException>());
        
    }
}