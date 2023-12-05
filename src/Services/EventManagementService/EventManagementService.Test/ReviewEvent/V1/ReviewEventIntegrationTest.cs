using Dapper;
using EventManagementService.Application.V1.ReviewEvent;
using EventManagementService.Application.V1.ReviewEvent.Repository;
using EventManagementService.Domain.Models.Events;
using EventManagementService.Infrastructure;
using EventManagementService.Infrastructure.AppSettings;
using EventManagementService.Infrastructure.EventBus;
using EventManagementService.Test.ReviewEvent.V1.Models;
using EventManagementService.Test.Shared;
using EventManagementService.Test.Shared.Builders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Npgsql;

namespace EventManagementService.Test.ReviewEvent.V1;

[TestFixture]
public class ReviewEventIntegrationTest
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
    public async Task CreateNewEventReview_SuccessfullyInsertReview_ThrowsNoException()
    {
        // Arrange
        var sqlRepoLogger = new Mock<ILogger<SqlReviewEvent>>();
        var busLogger = new Mock<ILogger<PubSubEventBus>>();
        var pubSubRepoLogger = new Mock<ILogger<PubSubReviewEvent>>();
        var handlerLogger = new Mock<ILogger<ReviewEventHandler>>();
        var eventBusMock = new Mock<IEventBus>();

        var pubsubConfig = new PubSub
        {
            Topics = new[]
            {
                new Topic()
                {
                    ProjectId = "test",
                    TopicId = "test"
                },
                new Topic()
                {
                    ProjectId = "test",
                    TopicId = "test"
                },
                new Topic()
                {
                    ProjectId = "test",
                    TopicId = "test"
                }
            },
            SubscriptionName = "test"
        };

        var handler = new ReviewEventHandler
        (
            new SqlReviewEvent(_connectionStringManager, sqlRepoLogger.Object),
            new PubSubReviewEvent(Options.Create(pubsubConfig), eventBusMock.Object, pubSubRepoLogger.Object),
            handlerLogger.Object
        );

        var builder = new DataBuilder(_connectionStringManager);
        var eEvent = builder.NewTestEvent();
        builder.InsertEvents(new[] { eEvent });

        var request = new ReviewEventRequest
        (
            new Review
            {
                Rate = 2,
                ReviewDate = DateTimeOffset.UtcNow,
                EventId = eEvent.Id,
                ReviewerId = "Test"
            }
        );

        // Act
        var act = async () => await handler.Handle(request, new CancellationToken());

        // Assert
        Assert.DoesNotThrowAsync(() => act.Invoke());
    }

    [Test]
    public async Task CreateNewEventReview_SuccessfullyInsertReview_ReturnsCreatedReviewFromDatabase()
    {
        // Arrange
        var sqlRepoLogger = new Mock<ILogger<SqlReviewEvent>>();
        var busLogger = new Mock<ILogger<PubSubEventBus>>();
        var pubSubRepoLogger = new Mock<ILogger<PubSubReviewEvent>>();
        var handlerLogger = new Mock<ILogger<ReviewEventHandler>>();
        var eventBusMock = new Mock<IEventBus>();

        var pubsubConfig = new PubSub
        {
            Topics = new[]
            {
                new Topic()
                {
                    ProjectId = "test",
                    TopicId = "test"
                },
                new Topic()
                {
                    ProjectId = "test",
                    TopicId = "test"
                },
                new Topic()
                {
                    ProjectId = "test",
                    TopicId = "test"
                }
            },
            SubscriptionName = "test"
        };

        var handler = new ReviewEventHandler
        (
            new SqlReviewEvent(_connectionStringManager, sqlRepoLogger.Object),
            new PubSubReviewEvent(Options.Create(pubsubConfig), eventBusMock.Object, pubSubRepoLogger.Object),
            handlerLogger.Object
        );

        var builder = new DataBuilder(_connectionStringManager);
        var eEvent = builder.NewTestEvent();
        builder.InsertEvents(new[] { eEvent });

        var request = new ReviewEventRequest
        (
            new Review
            {
                Rate = 2,
                ReviewDate = DateTimeOffset.UtcNow,
                EventId = eEvent.Id,
                ReviewerId = "Test"
            }
        );

        // Act
        await handler.Handle(request, new CancellationToken());
        var createdReview = GetNewlyCreatedEvent(request.Review.ReviewDate);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(request.Review.Rate, Is.EqualTo(createdReview.rate));
            Assert.That(request.Review.ReviewerId, Is.EqualTo(createdReview.reviewer_id));
        });
    }

    [Test]
    public async Task CreateNewEventReview_SuccessfullyInsertEventReview_ReturnsCreatedEventReviewFromDatabase()
    {
        // Arrange
        var sqlRepoLogger = new Mock<ILogger<SqlReviewEvent>>();
        var busLogger = new Mock<ILogger<PubSubEventBus>>();
        var pubSubRepoLogger = new Mock<ILogger<PubSubReviewEvent>>();
        var handlerLogger = new Mock<ILogger<ReviewEventHandler>>();
        var eventBusMock = new Mock<IEventBus>();

        var pubsubConfig = new PubSub
        {
            Topics = new[]
            {
                new Topic()
                {
                    ProjectId = "test",
                    TopicId = "test"
                },
                new Topic()
                {
                    ProjectId = "test",
                    TopicId = "test"
                },
                new Topic()
                {
                    ProjectId = "test",
                    TopicId = "test"
                }
            },
            SubscriptionName = "test"
        };

        var handler = new ReviewEventHandler
        (
            new SqlReviewEvent(_connectionStringManager, sqlRepoLogger.Object),
            new PubSubReviewEvent(Options.Create(pubsubConfig), eventBusMock.Object, pubSubRepoLogger.Object),
            handlerLogger.Object
        );

        var builder = new DataBuilder(_connectionStringManager);
        var eEvent = builder.NewTestEvent();
        builder.InsertEvents(new[] { eEvent });

        var request = new ReviewEventRequest
        (
            new Review
            {
                Rate = 2,
                ReviewDate = DateTimeOffset.UtcNow,
                EventId = eEvent.Id,
                ReviewerId = "Test"
            }
        );

        // Act
        await handler.Handle(request, new CancellationToken());
        var createdReview = GetNewlyCreatedEvent(request.Review.ReviewDate);
        var createdEventReview = GetEventReview(createdReview.id);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(request.Review.EventId, Is.EqualTo(createdEventReview.event_id));
            Assert.That(createdReview.id, Is.EqualTo(createdEventReview.review_id));
        });
    }

    private ReviewTable GetNewlyCreatedEvent(DateTimeOffset reviewDate)
    {
        using var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString());
        connection.OpenAsync();
        var sql = """
                  SELECT * FROM review WHERE review_date = @date
                  """;
        return connection.QueryFirstOrDefault<ReviewTable>(sql, new { date = reviewDate });
    }

    private EventReviewTable GetEventReview(int id)
    {
        using var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString());
        connection.OpenAsync();
        const string sql = """
                           SELECT * FROM event_review WHERE review_id = @reviewId
                           """;
        return connection.QueryFirstOrDefault<EventReviewTable>(sql, new { reviewId = id });
    }
}