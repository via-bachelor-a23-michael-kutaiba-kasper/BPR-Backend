using System.Transactions;
using Dapper;
using EventManagementService.Application.V1.ReviewEvent.Exceptions;
using EventManagementService.Domain.Models.Events;
using EventManagementService.Infrastructure;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace EventManagementService.Application.V1.ReviewEvent.Repository;

public interface ISqlReviewEvent
{
    Task<int> CreateEventReview(Review review);
    Task<bool> UserAlreadyMadeReview(Review review);
}

public class SqlReviewEvent : ISqlReviewEvent
{
    private readonly IConnectionStringManager _connectionStringManager;
    private readonly ILogger<SqlReviewEvent> _logger;

    public SqlReviewEvent
    (
        IConnectionStringManager connectionStringManager,
        ILogger<SqlReviewEvent> logger
    )
    {
        _connectionStringManager = connectionStringManager;
        _logger = logger;
    }

    public async Task<int> CreateEventReview(Review review)
    {
        await using var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString());
        await connection.OpenAsync();
        await using NpgsqlTransaction transaction = await connection.BeginTransactionAsync();
        try
        {
            var reviewId = await InsertNewReview(review, connection);
            await InsertNewEventReview(review.EventId, reviewId, connection);
            await transaction.CommitAsync();
            return reviewId;
        }
        catch (Exception e)
        {
            try
            {
                await transaction.RollbackAsync();
                _logger.LogInformation(e, "Inserting review transaction successfully rolled back");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Something went wrong while rolling back insert review transaction");
                throw new TransactionException("Cannot role back insert review transaction");
            }

            throw new InsertReviewException("Cannot insert review", e);
        }
    }

    public async Task<bool> UserAlreadyMadeReview(Review review)
    {
        await using var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString());
        await connection.OpenAsync();
        try
        {
            var hasReview = await connection.QueryFirstOrDefaultAsync<bool>(
                CheckUserReviewSql, 
                new { eventId = review.EventId, reviewerId = review.ReviewerId }
            );
            _logger.LogInformation($"Check if user reviewed an event returns: {hasReview}");
            return hasReview;
        }
        catch (Exception e)
        {
            _logger.LogError($"Failed to check user review{e.StackTrace}");
            throw new CheckUserEventReviewException("Cannot check if user reviewed an event", e);
        }
    }

    private static async Task InsertNewEventReview(int evId, int rvId, NpgsqlConnection connection)
    {
        await connection.ExecuteAsync(CreateEventReviewSql, new { eventId = evId, reviewId = rvId });
    }

    private static async Task<int> InsertNewReview(Review review, NpgsqlConnection connection)
    {
        var reviewId = await connection.ExecuteScalarAsync<int>(
            CreateReviewSql,
            new
            {
                rate = review.Rate,
                reviewerId = review.ReviewerId,
                reviewDate = review.ReviewDate
            }
        );
        return reviewId;
    }

    private const string CreateReviewSql =
        """
        INSERT INTO review(rate, reviewer_id, review_date)
        VALUES (@rate, @reviewerId, @reviewDate) RETURNING id;
        """;

    private const string CreateEventReviewSql =
        """
        INSERT INTO event_review(event_id, review_id)
        VALUES (@eventId, @reviewId);
        """;

    private const string CheckUserReviewSql =
        """
        SELECT COUNT(*) > 0 AS has_reviewed
        FROM event_review er
        JOIN review r ON er.review_id = r.id
        WHERE er.event_id = @eventId AND r.reviewer_id = @reviewerId;
        """;
}