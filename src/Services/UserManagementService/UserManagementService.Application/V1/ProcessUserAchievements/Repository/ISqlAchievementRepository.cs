using System.Transactions;
using Dapper;
using Microsoft.Extensions.Logging;
using Npgsql;
using UserManagementService.Application.V1.ProcessUserAchievements.Exceptions;
using UserManagementService.Application.V1.ProcessUserAchievements.Model;
using UserManagementService.Domain.Models.Events;
using UserManagementService.Domain.Util;
using UserManagementService.Infrastructure;

namespace UserManagementService.Application.V1.ProcessUserAchievements.Repository;

public interface ISqlAchievementRepository
{
    Task InsertUserAchievement(UserAchievementTable userAchievements);

    Task UpsertAchievementProgress(UnlockableAchievementProgressTable unlockableAchievementProgressTable);

    Task<IReadOnlyCollection<int>> GetUserProgress(string userId, int achievementId, Category eventCategory);
    Task<IReadOnlyCollection<UserAchievementJoinTable>?> GetUserAchievement(string userId);
    Task<IReadOnlyCollection<AchievementTable>> GetAchievements();
    Task<int> GetProgressForAnAchievement(string userId, int achievement);
}

public class SqlAchievementRepository : ISqlAchievementRepository
{
    private readonly IConnectionStringManager _connectionStringManager;
    private readonly ILogger<SqlAchievementRepository> _logger;

    public SqlAchievementRepository
    (
        IConnectionStringManager connectionStringManager,
        ILogger<SqlAchievementRepository> logger
    )
    {
        _connectionStringManager = connectionStringManager;
        _logger = logger;
    }

    public async Task InsertUserAchievement(UserAchievementTable userAchievements)
    {
        await using var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString());
        await connection.OpenAsync();
        await using NpgsqlTransaction transaction = await connection.BeginTransactionAsync();
        try
        {
            await connection.ExecuteAsync(InsertNewUserAchievementSql, new
            {
                achievement_id = userAchievements.achievement_id, user_id = userAchievements.user_id,
                unlocked_date = userAchievements.unlocked_date
            });

            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            try
            {
                await transaction.RollbackAsync();
                _logger.LogInformation(e, "Inserting user achievement transaction successfully rolled back");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Something went wrong while rolling back user achievement transaction");
                throw new TransactionException("Cannot role back insert user achievement transaction");
            }

            throw new InsertUserAchievementException("Cannot insert user achievement", e);
        }
    }

    public async Task UpsertAchievementProgress(UnlockableAchievementProgressTable unlockableAchievementProgressTable)
    {
        await using var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString());
        await connection.OpenAsync();
        await using NpgsqlTransaction transaction = await connection.BeginTransactionAsync();
        try
        {
            await connection.ExecuteAsync(UpsertUserAchievementProgressSql, new
            {
                achievementId = unlockableAchievementProgressTable.achievement_id,
                userId = unlockableAchievementProgressTable.user_id,
                progress = unlockableAchievementProgressTable.progress, date = unlockableAchievementProgressTable.date
            });


            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            try
            {
                await transaction.RollbackAsync();
                _logger.LogInformation(e, "Inserting user achievement progress transaction successfully rolled back");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex,
                    "Something went wrong while rolling back user achievement progress transaction");
                throw new TransactionException("Cannot role back insert user achievement progress transaction");
            }

            throw new InsertUserAchievementException("Cannot insert user achievement progress", e);
        }
    }

    public async Task<IReadOnlyCollection<int>> GetUserProgress(string userId, int achievementId,
        Category eventCategory)
    {
        await using var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString());
        await connection.OpenAsync();
        var currentProgress = await connection.QueryAsync<int>(GetUserAchievementSql,
            new { user_id = userId, achievement_id = achievementId });

        var categoryGroup = EnumCategoryGroupHelper.GetCategoryGroupAttribute(eventCategory);

        var achievementsToInsert = EnumCategoryGroupHelper.GetAchievementsForCategoryGroup(categoryGroup!.Group)
            .Where(achievement => !currentProgress.Contains((int)achievement))
            .ToList();

        if (!achievementsToInsert.Any())
        {
            // Insert initial records for achievements without existing progress
            foreach (var achievement in achievementsToInsert)
            {
                await connection.ExecuteAsync(
                    InsertInitialProgressSql,
                    new { UserId = userId, AchievementId = achievement, Progress = 0, Date = DateTimeOffset.UtcNow }
                );
            }
        }

        return currentProgress.ToList();
    }

    public async Task<IReadOnlyCollection<UserAchievementJoinTable>?> GetUserAchievement(string userId)
    {
        await using var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString());
        await connection.OpenAsync();
        try
        {
            var query =
                await connection.QueryAsync<UserAchievementJoinTable>(GetUserAchievementSql, new { user_id = userId });

            var userAchievementTables = query.ToList();
            _logger.LogInformation($"{userAchievementTables.Count()} user achievements retrieved form database");
            return userAchievementTables;
        }
        catch (Exception e)
        {
            _logger.LogError($"Unable to query user achievements, {e.StackTrace}");
            throw new QueryUserAchievementsException("Something went wrong while querying user achievement", e);
        }
    }

    public async Task<IReadOnlyCollection<AchievementTable>> GetAchievements()
    {
        await using var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString());
        await connection.OpenAsync();
        try
        {
            var query =
                await connection.QueryAsync<AchievementTable>(GetAchievementsSql);

            var achievements = query.ToList();

            _logger.LogInformation($"{achievements.Count()} achievements retrieved form database");
            return achievements;
        }
        catch (Exception e)
        {
            _logger.LogError($"Unable to query achievements, {e.StackTrace}");
            throw new QueryUserAchievementsException("Something went wrong while querying achievement", e);
        }
    }

    public async Task<int> GetProgressForAnAchievement(string userId, int achievement)
    {
        await using var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString());
        await connection.OpenAsync();
        try
        {
            var achievementProgerss = await connection.QueryFirstOrDefaultAsync<int>(GetPreogressForAnAchievementSql,
                new { UserId = userId, AchievementId = achievement });
            return achievementProgerss == default(int) ? 0 : achievementProgerss;
        }
        catch (Exception e)
        {
            _logger.LogError($"Unable to query achievement progress, {e.StackTrace}");
            throw new QueryUserAchievementsException("Something went wrong while querying achievement progress", e);
        }
    }

    private const string InsertNewUserAchievementSql =
        """
        INSERT INTO user_achievement(achievement_id, user_id, unlocked_date) VALUES (@achievement_id, @user_id, @unlocked_date);
        """;

    private const string GetUserAchievementSql =
        """
        SELECT * FROM user_achievement ua JOIN achievement a on a.id = ua.achievement_id WHERE user_id = @user_id;
        """;

    private const string GetAchievementsSql =
        """
        SELECT * FROM achievement
        """;

    private const string UpsertUserAchievementProgressSql =
        """
        INSERT INTO unlockable_achievement_progress(achievement_id, user_id, progress, date)
        VALUES (@achievementId, @userId, @progress, @date)
        ON CONFLICT (achievement_id, user_id)
            DO UPDATE SET progress = EXCLUDED.progress, date = EXCLUDED.date;
        """;

    private const string GetUSerProgressSql =
        """
        SELECT progress FROM unlockable_achievement_progress
        WHERE user_id = @UserId AND achievement_id IN @Achievements
        """;

    private const string InsertInitialProgressSql =
        """
        INSERT INTO unlockable_achievement_progress (user_id, achievement_id, progress, date)
        VALUES (@UserId, @AchievementId, @Progress, @Date)
        """;

    private const string GetPreogressForAnAchievementSql =
        """
        SELECT progress FROM unlockable_achievement_progress
        WHERE user_id = @UserId AND achievement_id = @AchievementId
        """;
}