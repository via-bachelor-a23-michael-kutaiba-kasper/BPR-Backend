using Dapper;
using EventManagementService.Application.V1.FetchReviewsByUser;
using EventManagementService.Application.V1.FetchReviewsByUser.Exceptions;
using EventManagementService.Application.V1.FetchReviewsByUser.Repository;
using EventManagementService.Domain.Models.Events;
using EventManagementService.Infrastructure;
using EventManagementService.Test.Shared;
using EventManagementService.Test.Shared.Builders;
using Microsoft.Extensions.Logging;
using Moq;
using Npgsql;

namespace EventManagementService.Test.FetchReviewsByUser.V1;

[TestFixture]
public class FetchReviewsByUserIntegrationTests
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
    public async Task FethEventsByUserId_Successful_ThrowsNoException()
    {
        // Arrange
        var sqlRepoLogger = new Mock<ILogger<SqlReviews>>();
        var handlerLogger = new Mock<ILogger<FetchReviewsByUSerHandler>>();

        var handler = new FetchReviewsByUSerHandler
        (
            new SqlReviews(_connectionStringManager, sqlRepoLogger.Object),
            handlerLogger.Object
        );

        var builder = new DataBuilder(_connectionStringManager);
        var eEvent = builder.NewTestEvent();
        builder.InsertEvents(new[] { eEvent });
        var review = new Review
        {
            EventId = eEvent.Id,
            ReviewDate = DateTimeOffset.UtcNow,
            ReviewerId = "Oq8tmUrDV6SeEpWf1olCJNJ1JW93",
            Rate = 3.5f
        };

        InsertReviewAndEventReview(review, eEvent.Id);

        // Act
        var act = async () => await handler.Handle(new FetchReviewsByUserRequest
            (
                "Oq8tmUrDV6SeEpWf1olCJNJ1JW93"
            )
            , new CancellationToken());

        // Assert
        Assert.DoesNotThrowAsync(()=> act.Invoke());
    }
    
    [Test]
    public async Task FethEventsByUserId_Successful_ReturnsCorrectEventReviews()
    {
        // Arrange
        var sqlRepoLogger = new Mock<ILogger<SqlReviews>>();
        var handlerLogger = new Mock<ILogger<FetchReviewsByUSerHandler>>();

        var handler = new FetchReviewsByUSerHandler
        (
            new SqlReviews(_connectionStringManager, sqlRepoLogger.Object),
            handlerLogger.Object
        );

        var builder = new DataBuilder(_connectionStringManager);
        var eEvent = builder.NewTestEvent();
        builder.InsertEvents(new[] { eEvent });
        var review = new Review
        {
            EventId = eEvent.Id,
            ReviewDate = DateTimeOffset.UtcNow,
            ReviewerId = "Oq8tmUrDV6SeEpWf1olCJNJ1JW93",
            Rate = 3.5f
        };
        
        var otherUserReview = new Review
        {
            EventId = eEvent.Id,
            ReviewDate = DateTimeOffset.UtcNow,
            ReviewerId = "Oq8tmDrDV6SeEpWf1olCJNJ1JW94",
            Rate = 2.5f
        };

        InsertReviewAndEventReview(review, eEvent.Id);
        InsertReviewAndEventReview(otherUserReview, eEvent.Id);

        // Act
        var userReviews= await handler.Handle(new FetchReviewsByUserRequest
            (
                "Oq8tmUrDV6SeEpWf1olCJNJ1JW93"
            )
            , new CancellationToken());

        // Assert
        foreach (var ur in userReviews)
        {
            Assert.That(ur.UserId, Is.EqualTo(review.ReviewerId));
            Assert.That(ur.UserId, Is.Not.EqualTo(otherUserReview.ReviewerId));
        }
    }
    
    
    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public async Task FethEventsByUserId_WithInvalidUserId_ThrowsInvalidUserIdException(string userId)
    {
        // Arrange
        var sqlRepoLogger = new Mock<ILogger<SqlReviews>>();
        var handlerLogger = new Mock<ILogger<FetchReviewsByUSerHandler>>();

        var handler = new FetchReviewsByUSerHandler
        (
            new SqlReviews(_connectionStringManager, sqlRepoLogger.Object),
            handlerLogger.Object
        );

        var builder = new DataBuilder(_connectionStringManager);
        var eEvent = builder.NewTestEvent();
        builder.InsertEvents(new[] { eEvent });
        var review = new Review
        {
            EventId = eEvent.Id,
            ReviewDate = DateTimeOffset.UtcNow,
            ReviewerId = "Oq8tmUrDV6SeEpWf1olCJNJ1JW93",
            Rate = 3.5f
        };

        InsertReviewAndEventReview(review, eEvent.Id);

        // Act
        var act = async () => await handler.Handle(new FetchReviewsByUserRequest
            (
                userId
            )
            , new CancellationToken());
        var exception = Assert.ThrowsAsync<FetchReviewsByUserException>(() => act.Invoke());

        // Assert
        Assert.That(exception!.InnerException!, Is.TypeOf<InvalidUserIdException>());
    }


    private void InsertReviewAndEventReview(Review review, int evtId)
    {
        using var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString());
        connection.OpenAsync();
        const string reviewSql =
            """
            INSERT INTO review(rate, reviewer_id, review_date)
            VALUES (@rate, @reviewerId, @reviewDate) RETURNING id;
            """;

        var rvId = connection.ExecuteScalar<int>(reviewSql, new
        {
            rate = review.Rate,
            reviewerId = review.ReviewerId,
            reviewDate = review.ReviewDate
        });

        const string eventReviewSql =
            """
            INSERT INTO event_review(event_id, review_id)
            VALUES (@eventId, @reviewId);
            """;
        connection.Execute(eventReviewSql, new
        {
            eventId = evtId,
            reviewId = rvId
        });
    }
}