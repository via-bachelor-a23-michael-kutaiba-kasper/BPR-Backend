using Dapper;
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

    public ReviewRepository(IEventBus eventBus, IOptions<PubSub> pubsubConfig, IConnectionStringManager connectionStringManager)
    {
        _eventBus = eventBus;
        _pubsubConfig = pubsubConfig;
        _connectionStringManager = connectionStringManager;
    }

    public async Task<IReadOnlyCollection<Review>> GetNewReviews()
    {
        var topic = _pubsubConfig.Value.Topics[PubSubTopics.VibeVerseEventsNewReview];
        var newReviews = await _eventBus.PullAsync<Review>(topic.TopicId, topic.ProjectId,
            _pubsubConfig.Value.SubscriptionName, 1000, new CancellationToken());

        return newReviews.ToList();
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