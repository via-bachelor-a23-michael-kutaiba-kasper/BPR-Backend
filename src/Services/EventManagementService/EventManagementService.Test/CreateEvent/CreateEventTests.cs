using System.Collections;
using EventManagementService.Application.CreateEvent;
using EventManagementService.Application.CreateEvent.Exceptions;
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
            Keywords = new List<Keyword>
            {
                Keyword.ClassicalPerformance,
                Keyword.Basketball,
                Keyword.ArtExhibition
            },
            Url = "http://test.com/events/1",
            AdultsOnly = false,
            CreatedDate = DateTimeOffset.UtcNow,
            StartDate = DateTimeOffset.UtcNow.AddDays(1),
            EndDate = DateTimeOffset.UtcNow.AddDays(2).AddHours(2),
            AccessCode = "321km3lkmdkslajdkas321",
            Host = new User
            {
                UserId = "Oq8tmHrYV6SeEpWf1olCJNJ1JW93",
                DisplayName = "Test",
                CreationDate = DateTimeOffset.UtcNow
            },
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
        var act = async () => await handler.Handle(request, new CancellationToken());

        // Assert 
        Assert.DoesNotThrowAsync(() => act.Invoke());
    }

    [TestCase("")]
    [TestCase("  ")]
    [TestCase(null)]
    public async Task CreateNewEvent_WithInvalidTitle_ThorwEventValidationException(string title)
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
            Title = title,
            Keywords = new List<Keyword>
            {
                Keyword.ClassicalPerformance,
                Keyword.Basketball,
                Keyword.ArtExhibition
            },
            Url = "http://test.com/events/1",
            AdultsOnly = false,
            CreatedDate = DateTimeOffset.UtcNow,
            StartDate = DateTimeOffset.UtcNow.AddDays(1),
            EndDate = DateTimeOffset.UtcNow.AddDays(2).AddHours(2),
            AccessCode = "321km3lkmdkslajdkas321",
            Host = new User
            {
                UserId = "Oq8tmHrYV6SeEpWf1olCJNJ1JW93",
                DisplayName = "Test",
                CreationDate = DateTimeOffset.UtcNow
            },
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
        var act = async () => await handler.Handle(request, new CancellationToken());
        var exception = Assert.ThrowsAsync<CreateEventException>(() => act.Invoke());
        var validationException = (EventValidationException)exception!.InnerException!;

        // Assert 
        Assert.That(validationException.Message, Is.EqualTo("Event is missing Title"));
    }


    [Test]
    public async Task CreateNewEvent_WithInvalidStartDate_ThorwEventValidationException()
    {
        // Arrange
        var startDate = new DateTimeOffset(2022, 1, 1, 0, 0, 0, TimeSpan.Zero);

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
            Title = "Test",
            Keywords = new List<Keyword>
            {
                Keyword.ClassicalPerformance,
                Keyword.Basketball,
                Keyword.ArtExhibition
            },
            Url = "http://test.com/events/1",
            AdultsOnly = false,
            CreatedDate = DateTimeOffset.UtcNow,
            StartDate = startDate,
            EndDate = DateTimeOffset.UtcNow.AddDays(2).AddHours(2),
            AccessCode = "321km3lkmdkslajdkas321",
            Host = new User
            {
                UserId = "Oq8tmHrYV6SeEpWf1olCJNJ1JW93",
                DisplayName = "Test",
                CreationDate = DateTimeOffset.UtcNow
            },
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
        var act = async () => await handler.Handle(request, new CancellationToken());
        var exception = Assert.ThrowsAsync<CreateEventException>(() => act.Invoke());
        var validationException = (EventValidationException)exception!.InnerException!;

        // Assert 
        Assert.That(validationException.Message, Is.EqualTo("Event start date is either null or is in the past"));
    }

    [Test]
    public async Task CreateNewEvent_WithInvalidEndDate_ThorwEventValidationException()
    {
        // Arrange
        var endDate = new DateTimeOffset(2022, 1, 1, 0, 0, 0, TimeSpan.Zero);
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
            Title = "Test",
            Keywords = new List<Keyword>
            {
                Keyword.ClassicalPerformance,
                Keyword.Basketball,
                Keyword.ArtExhibition
            },
            Url = "http://test.com/events/1",
            AdultsOnly = false,
            CreatedDate = DateTimeOffset.UtcNow,
            StartDate = DateTimeOffset.UtcNow.AddDays(1),
            EndDate = endDate,
            AccessCode = "321km3lkmdkslajdkas321",
            Host = new User
            {
                UserId = "Oq8tmHrYV6SeEpWf1olCJNJ1JW93",
                DisplayName = "Test",
                CreationDate = DateTimeOffset.UtcNow
            },
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
        var act = async () => await handler.Handle(request, new CancellationToken());
        var exception = Assert.ThrowsAsync<CreateEventException>(() => act.Invoke());
        var validationException = (EventValidationException)exception!.InnerException!;

        // Assert 
        Assert.That(validationException.Message,
            Is.EqualTo("Event end date is either null or older than start or current date"));
    }

    [Test]
    public async Task CreateNewEvent_WithInvalidCreateDate_ThorwEventValidationException()
    {
        // Arrange
        var createDate = new DateTimeOffset(2022, 1, 1, 0, 0, 0, TimeSpan.Zero);
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
            Title = "Test",
            Keywords = new List<Keyword>
            {
                Keyword.ClassicalPerformance,
                Keyword.Basketball,
                Keyword.ArtExhibition
            },
            Url = "http://test.com/events/1",
            AdultsOnly = false,
            CreatedDate = createDate,
            StartDate = DateTimeOffset.UtcNow.AddDays(1),
            EndDate = DateTimeOffset.UtcNow.AddDays(2).AddHours(2),
            AccessCode = "321km3lkmdkslajdkas321",
            Host = new User
            {
                UserId = "Oq8tmHrYV6SeEpWf1olCJNJ1JW93",
                DisplayName = "Test",
                CreationDate = DateTimeOffset.UtcNow
            },
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
        var act = async () => await handler.Handle(request, new CancellationToken());
        var exception = Assert.ThrowsAsync<CreateEventException>(() => act.Invoke());
        var validationException = (EventValidationException)exception!.InnerException!;

        // Assert 
        Assert.That(validationException.Message,
            Is.EqualTo("Event created date is either null or greater than end or start dates"));
    }

    [TestCase("")]
    [TestCase("  ")]
    [TestCase(null)]
    public async Task CreateNewEvent_WithInvalidHostId_ThorwEventValidationException(string hostId)
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
            Title = "Test",
            Keywords = new List<Keyword>
            {
                Keyword.ClassicalPerformance,
                Keyword.Basketball,
                Keyword.ArtExhibition
            },
            Url = "http://test.com/events/1",
            AdultsOnly = false,
            CreatedDate = DateTimeOffset.UtcNow,
            StartDate = DateTimeOffset.UtcNow.AddDays(1),
            EndDate = DateTimeOffset.UtcNow.AddDays(2).AddHours(2),
            AccessCode = "321km3lkmdkslajdkas321",
            Host = new User
            {
                UserId = hostId,
                DisplayName = "Test",
                CreationDate = DateTimeOffset.UtcNow
            },
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
        var act = async () => await handler.Handle(request, new CancellationToken());
        var exception = Assert.ThrowsAsync<CreateEventException>(() => act.Invoke());
        var validationException = (EventValidationException)exception!.InnerException!;

        // Assert 
        Assert.That(validationException.Message, Is.EqualTo("Host id is either null or empty"));
    }

    [TestCase("")]
    [TestCase("  ")]
    [TestCase(null)]
    public async Task CreateNewEvent_WithInvalidLocation_ThorwEventValidationException(string location)
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
            Location = location,
            Category = Category.Music,
            Images = new List<string>(),
            Title = "Test",
            Keywords = new List<Keyword>
            {
                Keyword.ClassicalPerformance,
                Keyword.Basketball,
                Keyword.ArtExhibition
            },
            Url = "http://test.com/events/1",
            AdultsOnly = false,
            CreatedDate = DateTimeOffset.UtcNow,
            StartDate = DateTimeOffset.UtcNow.AddDays(1),
            EndDate = DateTimeOffset.UtcNow.AddDays(2).AddHours(2),
            AccessCode = "321km3lkmdkslajdkas321",
            Host = new User
            {
                UserId = "Test",
                DisplayName = "Test",
                CreationDate = DateTimeOffset.UtcNow
            },
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
        var act = async () => await handler.Handle(request, new CancellationToken());
        var exception = Assert.ThrowsAsync<CreateEventException>(() => act.Invoke());
        var validationException = (EventValidationException)exception!.InnerException!;

        // Assert 
        Assert.That(validationException.Message, Is.EqualTo("Event location is either null or empty"));
    }


    [TestCase("")]
    [TestCase("  ")]
    [TestCase(null)]
    public async Task CreateNewEvent_WithInvalidCity_ThorwEventValidationException(string city)
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
            Location = "location",
            Category = Category.Music,
            Images = new List<string>(),
            Title = "Test",
            Keywords = new List<Keyword>
            {
                Keyword.ClassicalPerformance,
                Keyword.Basketball,
                Keyword.ArtExhibition
            },
            Url = "http://test.com/events/1",
            AdultsOnly = false,
            CreatedDate = DateTimeOffset.UtcNow,
            StartDate = DateTimeOffset.UtcNow.AddDays(1),
            EndDate = DateTimeOffset.UtcNow.AddDays(2).AddHours(2),
            AccessCode = "321km3lkmdkslajdkas321",
            Host = new User
            {
                UserId = "Test",
                DisplayName = "Test",
                CreationDate = DateTimeOffset.UtcNow
            },
            IsPaid = true,
            IsPrivate = false,
            MaxNumberOfAttendees = 200,
            LastUpdateDate = DateTimeOffset.UtcNow,
            GeoLocation = new GeoLocation
            {
                Lat = 0,
                Lng = 0
            },
            City = city,
            Description = "Test"
        });

        // Act
        var act = async () => await handler.Handle(request, new CancellationToken());
        var exception = Assert.ThrowsAsync<CreateEventException>(() => act.Invoke());
        var validationException = (EventValidationException)exception!.InnerException!;

        // Assert 
        Assert.That(validationException.Message, Is.EqualTo("Event city is either null or empty"));
    }

    [TestCase("")]
    [TestCase("  ")]
    [TestCase(null)]
    public async Task CreateNewEvent_WithInvalidHostDisplayName_ThorwEventValidationException(string displayName)
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
            Location = "location",
            Category = Category.Music,
            Images = new List<string>(),
            Title = "Test",
            Keywords = new List<Keyword>
            {
                Keyword.ClassicalPerformance,
                Keyword.Basketball,
                Keyword.ArtExhibition
            },
            Url = "http://test.com/events/1",
            AdultsOnly = false,
            CreatedDate = DateTimeOffset.UtcNow,
            StartDate = DateTimeOffset.UtcNow.AddDays(1),
            EndDate = DateTimeOffset.UtcNow.AddDays(2).AddHours(2),
            AccessCode = "321km3lkmdkslajdkas321",
            Host = new User
            {
                UserId = "Test",
                DisplayName = displayName,
                CreationDate = DateTimeOffset.UtcNow
            },
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
        var act = async () => await handler.Handle(request, new CancellationToken());
        var exception = Assert.ThrowsAsync<CreateEventException>(() => act.Invoke());
        var validationException = (EventValidationException)exception!.InnerException!;

        // Assert 
        Assert.That(validationException.Message, Is.EqualTo("Host display name is either null of empty"));
    }

    [TestCase(-91.0f)]
    [TestCase(91.0f)]
    public async Task CreateNewEvent_WithInvalidGeoLat_ThorwEventValidationException(float lat)
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
            Location = "location",
            Category = Category.Music,
            Images = new List<string>(),
            Title = "Test",
            Keywords = new List<Keyword>
            {
                Keyword.ClassicalPerformance,
                Keyword.Basketball,
                Keyword.ArtExhibition
            },
            Url = "http://test.com/events/1",
            AdultsOnly = false,
            CreatedDate = DateTimeOffset.UtcNow,
            StartDate = DateTimeOffset.UtcNow.AddDays(1),
            EndDate = DateTimeOffset.UtcNow.AddDays(2).AddHours(2),
            AccessCode = "321km3lkmdkslajdkas321",
            Host = new User
            {
                UserId = "Test",
                DisplayName = "displayName",
                CreationDate = DateTimeOffset.UtcNow
            },
            IsPaid = true,
            IsPrivate = false,
            MaxNumberOfAttendees = 200,
            LastUpdateDate = DateTimeOffset.UtcNow,
            GeoLocation = new GeoLocation
            {
                Lat = lat,
                Lng = 0
            },
            City = "Horsens",
            Description = "Test"
        });

        // Act
        var act = async () => await handler.Handle(request, new CancellationToken());
        var exception = Assert.ThrowsAsync<CreateEventException>(() => act.Invoke());
        var validationException = (EventValidationException)exception!.InnerException!;

        // Assert 
        Assert.That(validationException.Message, Is.EqualTo("Event geo location latitude is invalid"));
    }

    [TestCase(-181.0f)]
    [TestCase(181.0f)]
    public async Task CreateNewEvent_WithInvalidGeoLng_ThorwEventValidationException(float lng)
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
            Location = "location",
            Category = Category.Music,
            Images = new List<string>(),
            Title = "Test",
            Keywords = new List<Keyword>
            {
                Keyword.ClassicalPerformance,
                Keyword.Basketball,
                Keyword.ArtExhibition
            },
            Url = "http://test.com/events/1",
            AdultsOnly = false,
            CreatedDate = DateTimeOffset.UtcNow,
            StartDate = DateTimeOffset.UtcNow.AddDays(1),
            EndDate = DateTimeOffset.UtcNow.AddDays(2).AddHours(2),
            AccessCode = "321km3lkmdkslajdkas321",
            Host = new User
            {
                UserId = "Test",
                DisplayName = "displayName",
                CreationDate = DateTimeOffset.UtcNow
            },
            IsPaid = true,
            IsPrivate = false,
            MaxNumberOfAttendees = 200,
            LastUpdateDate = DateTimeOffset.UtcNow,
            GeoLocation = new GeoLocation
            {
                Lat = 0,
                Lng = lng
            },
            City = "Horsens",
            Description = "Test"
        });

        // Act
        var act = async () => await handler.Handle(request, new CancellationToken());
        var exception = Assert.ThrowsAsync<CreateEventException>(() => act.Invoke());
        var validationException = (EventValidationException)exception!.InnerException!;

        // Assert 
        Assert.That(validationException.Message, Is.EqualTo("Event geo location longitude is invalid"));
    }

    [Test]
    public async Task CreateNewEvent_WithInvalidMinNumberOfKeywords_ThorwEventValidationException()
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
            Location = "location",
            Category = Category.Music,
            Images = new List<string>(),
            Title = "Test",
            Keywords = new List<Keyword>
            {
                Keyword.ClassicalPerformance,
                Keyword.Basketball
            },
            Url = "http://test.com/events/1",
            AdultsOnly = false,
            CreatedDate = DateTimeOffset.UtcNow,
            StartDate = DateTimeOffset.UtcNow.AddDays(1),
            EndDate = DateTimeOffset.UtcNow.AddDays(2).AddHours(2),
            AccessCode = "321km3lkmdkslajdkas321",
            Host = new User
            {
                UserId = "Test",
                DisplayName = "displayName",
                CreationDate = DateTimeOffset.UtcNow
            },
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
        var act = async () => await handler.Handle(request, new CancellationToken());
        var exception = Assert.ThrowsAsync<CreateEventException>(() => act.Invoke());
        var validationException = (EventValidationException)exception!.InnerException!;

        // Assert 
        Assert.That(validationException.Message, Is.EqualTo("Event keywords are either less or greater that the required"));
    }
    
    [Test]
    public async Task CreateNewEvent_WithInvalidMaxNumberOfKeywords_ThorwEventValidationException()
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
            Location = "location",
            Category = Category.Music,
            Images = new List<string>(),
            Title = "Test",
            Keywords = new List<Keyword>
            {
                Keyword.ClassicalPerformance,
                Keyword.Basketball,
                Keyword.Beer,
                Keyword.Blues,
                Keyword.Coding,
                Keyword.Football
            },
            Url = "http://test.com/events/1",
            AdultsOnly = false,
            CreatedDate = DateTimeOffset.UtcNow,
            StartDate = DateTimeOffset.UtcNow.AddDays(1),
            EndDate = DateTimeOffset.UtcNow.AddDays(2).AddHours(2),
            AccessCode = "321km3lkmdkslajdkas321",
            Host = new User
            {
                UserId = "Test",
                DisplayName = "displayName",
                CreationDate = DateTimeOffset.UtcNow
            },
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
        var act = async () => await handler.Handle(request, new CancellationToken());
        var exception = Assert.ThrowsAsync<CreateEventException>(() => act.Invoke());
        var validationException = (EventValidationException)exception!.InnerException!;

        // Assert 
        Assert.That(validationException.Message, Is.EqualTo("Event keywords are either less or greater that the required"));
    }
}