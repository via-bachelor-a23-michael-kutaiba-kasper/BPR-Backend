using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using UserManagementService.Application.V1.ProcessExpProgress.Data;
using UserManagementService.Domain.Models;
using UserManagementService.Infrastructure;
using UserManagementService.Infrastructure.AppSettings;
using UserManagementService.Infrastructure.PubSub;

namespace UserManagementService.Application.V1.ProcessExpProgress.Repository;

public interface IReviewRepository
{
    public Task<IReadOnlyCollection<Review>> GetNewReviews();
    public Task<int> GetReviewsCountByUser(string userId);
}

public class ReviewRepository : IReviewRepository
{
    private readonly IEventBus _eventBus;
    private readonly IOptions<PubSub> _pubsubConfig;
    private readonly IConnectionStringManager _connectionStringManager;
    private readonly ILogger<ReviewRepository> _logger;

    public ReviewRepository(IEventBus eventBus, IOptions<PubSub> pubsubConfig,
        IConnectionStringManager connectionStringManager, ILogger<ReviewRepository> logger)
    {
        _eventBus = eventBus;
        _pubsubConfig = pubsubConfig;
        _connectionStringManager = connectionStringManager;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<Review>> GetNewReviews()
    {
        try
        {
            _logger.LogInformation("Retrieving new reviews from PubSub");

            var topic = _pubsubConfig.Value.Topics[PubSubTopics.VibeVerseEventsNewReview];
            var newReviews = (await _eventBus.PullAsync<Review>(topic.TopicId, topic.ProjectId,
                topic.SubscriptionNames[PubSubSubscriptionNames.Exp], 1000, new CancellationToken())).ToList();

            _logger.LogInformation($"Retrieved {newReviews.Count} new reviews from PubSub");

            return newReviews.ToList();
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to fetch new reviews from PubSub");
            _logger.LogError(e.Message);
            _logger.LogError(e.StackTrace);
            return new List<Review>();
        }
    }

    public async Task<int> GetReviewsCountByUser(string userId)
    {
        await using var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString());

        var latestStatsEntryQuery = """
        SELECT * FROM user_progress.user_stats_history WHERE user_id=@userId ORDER BY datetime DESC LIMIT 1; 
        """;
        var latestStatsEntry = await connection.QueryFirstOrDefaultAsync<UserStatsHistoryEntity>(latestStatsEntryQuery,
            new
            {
                @userId = userId
            }) ?? new UserStatsHistoryEntity
        {
            user_id = userId,
            events_hosted = 0,
            datetime = DateTimeOffset.UtcNow,
            id = -1,
            reviews_created = 0
        };

        return latestStatsEntry.reviews_created;
    }
}