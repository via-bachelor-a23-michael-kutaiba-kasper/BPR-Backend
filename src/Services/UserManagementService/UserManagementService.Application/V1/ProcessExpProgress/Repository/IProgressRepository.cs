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
        await connection.OpenAsync();

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

        await connection.CloseAsync();
    }

    public async Task AddExpToUserProgressAsync(string userId, long exp)
    {
        await using var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString());
        await connection.OpenAsync();

        var insertProgressStatement =
            "INSERT INTO user_progress.user_exp_progress(user_id, exp_gained, datetime) VALUES (@userId, @expGained, @datetime)";
        await connection.ExecuteAsync(insertProgressStatement, new
        {
            @userId = userId,
            @expGained = exp,
            @datetime = DateTimeOffset.UtcNow
        });

        var selectProgressQuery = "SELECT * FROM user_progress.progress WHERE user_id = @userId";
        var progressEntity = await connection.QueryFirstAsync<ProgressEntity>(selectProgressQuery, new
        {
            @userId = userId
        });

        var updateProgressStatement =
            "UPDATE user_progress.progress SET total_exp = @totalExp WHERE user_id = @userId ";
        await connection.ExecuteAsync(updateProgressStatement, new
        {
            @totalExp = progressEntity.total_exp + exp,
            @userId = userId
        });

        await connection.CloseAsync();
    }

    public async Task RegisterNewEventsHostedCount(string userId, int newEventsCount)
    {
        await using var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString());
        await connection.OpenAsync();

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
            reviews_created = 0,
            datetime = DateTimeOffset.UtcNow,
            id = -1
        };
        var newEventsHostedCount = latestStatsEntry.events_hosted + newEventsCount;

        var updateQuery = """
        INSERT INTO user_progress.user_stats_history(user_id,reviews_created, events_hosted, datetime)  VALUES (@userId, @reviewsCreated, @eventsHosted, @datetime)
        """;
        await connection.ExecuteAsync(updateQuery, new
        {
            @userId = userId,
            @reviewsCreated = latestStatsEntry.reviews_created,
            @eventsHosted = newEventsHostedCount,
            @datetime = DateTimeOffset.UtcNow
        });

        await connection.CloseAsync();
    }

    public async Task RegisterNewReviewCount(string userId, int newReviewCount)
    {
        await using var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString());
        await connection.OpenAsync();

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
        var newReviewsCount = latestStatsEntry.reviews_created + newReviewCount;

        var updateQuery = """
        INSERT INTO user_progress.user_stats_history(user_id,reviews_created, events_hosted, datetime)  VALUES (@userId, @reviewsCreated, @eventsHosted, @datetime)
        """;
        await connection.ExecuteAsync(updateQuery, new
        {
            @userId = userId,
            @reviewsCreated = newReviewsCount,
            @eventsHosted = latestStatsEntry.events_hosted,
            @datetime = DateTimeOffset.UtcNow
        });

        await connection.CloseAsync();
    }
}