using Dapper;
using Npgsql;
using UserManagementService.Application.V1.ProcessExpProgress.Data;
using UserManagementService.Infrastructure;
using UserManagementService.Infrastructure.Util;

namespace UserManagementService.Application.V1.ProcessExpProgress.Repository;

public interface IProgressRepository
{
    public Task EnsureUserHasProgress(string userId);
    public Task AddExpToUserProgressAsync(string userId, long exp);
    public Task RegisterNewEventsHostedCount(string userId, int newEventsCount);
    public Task RegisterNewReviewCount(string userId, int newReviewCount);
}

public class ProgressRepository : IProgressRepository
{
    private readonly IConnectionStringManager _connectionStringManager;

    public ProgressRepository(IConnectionStringManager connectionStringManager)
    {
        _connectionStringManager = connectionStringManager;
    }

    public async Task EnsureUserHasProgress(string userId)
    {
        await using var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString());
        var getProgressQuery = "SELECT * FROM user_progress.progress WHERE user_id = @userId";
        var existingEntity = await connection.QueryFirstOrDefaultAsync<ProgressEntity>(getProgressQuery, new
        {
            @userId = userId
        });

        if (existingEntity is null)
        {
            var insertProgressStatement =
                "INSERT INTO user_progress.progress (user_id, total_exp, stage) VALUES (@userId, 0, 1)";
            await connection.ExecuteAsync(insertProgressStatement, new
            {
                @userId = userId
            });
        }
    }

    public Task AddExpToUserProgressAsync(string userId, long exp)
    {
        throw new NotImplementedException();
    }

    public async Task RegisterNewEventsHostedCount(string userId, int newEventsCount)
    {
        var latestStatsEntryQuery = """
        SELECT * FROM user_progress.user_stats_history WHERE user_id=@userId ORDER BY datetime DESC LIMIT 1; 
        """;

        await using var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString());
        await connection.OpenAsync();

        var latestStatsEntry = await connection.QueryFirstAsync<UserStatsHistoryEntity>(latestStatsEntryQuery, new
        {
            @userId = userId
        });
        var newEventsHostedCount = latestStatsEntry.events_hosted + newEventsCount;

        var updateQuery = """
        INSERT INTO user_progress.user_stats_history(user_id,reviews_created, events_hosted, datetime)  VALUES (@userId, @reviewsCreated, @eventsHosted, @datetime)
        """;
        await connection.ExecuteAsync(updateQuery, new
        {
            @userId = userId,
            @reviewsCreated = latestStatsEntry.reviews_created,
            @eventsHosted = newEventsHostedCount,
            @datetime = DateTimeOffset.UtcNow.ToFormattedUtcString()
        });
    }

    public async Task RegisterNewReviewCount(string userId, int newReviewCount)
    {
        var latestStatsEntryQuery = """
        SELECT * FROM user_progress.user_stats_history WHERE user_id=@userId ORDER BY datetime DESC LIMIT 1; 
        """;

        await using var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString());
        await connection.OpenAsync();

        var latestStatsEntry = await connection.QueryFirstAsync<UserStatsHistoryEntity>(latestStatsEntryQuery, new
        {
            @userId = userId
        });
        var newReviewsCount = latestStatsEntry.reviews_created + newReviewCount;

        var updateQuery = """
        INSERT INTO user_progress.user_stats_history(user_id,reviews_created, events_hosted, datetime)  VALUES (@userId, @reviewsCreated, @eventsHosted, @datetime)
        """;
        await connection.ExecuteAsync(updateQuery, new
        {
            @userId = userId,
            @reviewsCreated = newReviewsCount,
            @eventsHosted = latestStatsEntry.events_hosted,
            @datetime = DateTimeOffset.UtcNow.ToFormattedUtcString()
        });
    }
}