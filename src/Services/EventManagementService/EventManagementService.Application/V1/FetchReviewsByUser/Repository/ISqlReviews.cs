using Dapper;
using EventManagementService.Application.V1.FetchReviewsByUser.Exceptions;
using EventManagementService.Application.V1.FetchReviewsByUser.Model;
using EventManagementService.Infrastructure;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace EventManagementService.Application.V1.FetchReviewsByUser.Repository;

public interface ISqlReviews
{
    Task<IReadOnlyCollection<EventReview>> FetchReviewsByUserId(string userId);
}

public class SqlReviews : ISqlReviews
{
    private readonly IConnectionStringManager _connectionStringManager;
    private readonly ILogger<SqlReviews> _logger;

    public SqlReviews
    (
        IConnectionStringManager connectionStringManager,
        ILogger<SqlReviews> logger
    )
    {
        _connectionStringManager = connectionStringManager;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<EventReview>> FetchReviewsByUserId(string userId)
    {
        await using var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString());
        await connection.OpenAsync();
        try
        {
            var query = await connection.QueryAsync<EventReview>
            (
                GetEventReviewsByUserId,
                new { reviewerId = userId }
            );
            
            _logger.LogInformation($"{query.Count()} event reviews for user: {userId} has been queried successfully");
            return query.ToList();
        }
        catch (Exception e)
        {
            _logger.LogError($"Unable to query event reviews: {e.StackTrace}");
            throw new CannotQueryEventReviewsException("Failed to query event reviews", e);
        }
    }

    private const string GetEventReviewsByUserId =
        """
        SELECT r.id AS ReviewId,
               r.rate AS Rate,
               r.reviewer_id AS UserId,
               r.review_date AS ReviewDate,
               er.event_id AS EventId
        FROM review r
            JOIN event_review er on r.id = er.review_id
                 WHERE r.reviewer_id = @reviewerId
        """;
}