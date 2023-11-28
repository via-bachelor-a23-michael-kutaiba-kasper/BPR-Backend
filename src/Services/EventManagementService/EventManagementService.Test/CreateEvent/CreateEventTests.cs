

using EventManagementService.Application.CreateEvent;
using EventManagementService.Application.CreateEvent.Repository;
using EventManagementService.Domain.Models;
using EventManagementService.Domain.Models.Events;
using EventManagementService.Infrastructure;
using EventManagementService.Test.Shared;
using Microsoft.Extensions.Logging;
using Moq;

namespace EventManagementService.Test.CreateEvent;

[TestFixture]
public class CreateEventTests
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
    public async Task CreateNewEvent_SuccessfullyInsertNewEvenInDatabase_ThrowsNoExceptions()
    {
        // Arrange
        var repoLogger = new Mock<ILogger<SqlCreateEvent>>();
        var handlerLogger = new Mock<ILogger<CreateEventHandler>>();
        var handler = new CreateEventHandler
        (
            new SqlCreateEvent
            (
                _connectionStringManager,
                repoLogger.Object
            ),
            handlerLogger.Object);

        var request = new CreateEventRequest(new Event
        {
            Id = 1,
            Location = "Vejlevej 14, 8700 Horsens, Denmark",
            Category = Category.Music,
            Images = new List<string>(),
            Title = "Beethoven Concerto",
            Keywords = new List<Keyword> { Keyword.ClassicalPerformance },
            Url = "http://test.com/events/1",
            AdultsOnly = false,
            CreatedDate = DateTimeOffset.UtcNow,
            StartDate = DateTimeOffset.UtcNow.AddDays(1),
            EndDate = DateTimeOffset.UtcNow.AddDays(1).AddHours(2),
            AccessCode = "321km3lkmdkslajdkas321",
            Host = new User { UserId = "Oq8tmHrYV6SeEpWf1olCJNJ1JW93" },
            IsPaid = true,
            IsPrivate = false,
            MaxNumberOfAttendees = 200,
            LastUpdateDate = DateTimeOffset.UtcNow,
            GeoLocation = new GeoLocation
            {
                Lat = 0,
                Lng = 0
            },
            City = "Horsens",
            Description = "Test"
        });

        // Act
        var act = async ()=> await handler.Handle(request, new CancellationToken());
        
        // Assert 
        Assert.DoesNotThrowAsync(()=> act.Invoke());
    }
}