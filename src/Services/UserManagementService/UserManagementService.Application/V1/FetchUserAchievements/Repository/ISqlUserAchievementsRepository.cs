using Dapper;
using Microsoft.Extensions.Logging;
using Npgsql;
using UserManagementService.Application.V1.FetchUserAchievements.Exceptions;
using UserManagementService.Application.V1.FetchUserAchievements.Model;
using UserManagementService.Domain.Models;
using UserManagementService.Infrastructure;

namespace UserManagementService.Application.V1.FetchUserAchievements.Repository;

public interface ISqlUserAchievementsRepository
{
    Task<IReadOnlyCollection<Achievement>> FetchUserAchievementByUserId(string userId);
}

public class SqlUserAchievementsRepository : ISqlUserAchievementsRepository
{
    private readonly ILogger<SqlUserAchievementsRepository> _logger;
    private readonly IConnectionStringManager _connectionStringManager;

    public SqlUserAchievementsRepository(ILogger<SqlUserAchievementsRepository> logger, IConnectionStringManager connectionStringManager)
    {
        _logger = logger;
        _connectionStringManager = connectionStringManager;
    }

    public async Task<IReadOnlyCollection<Achievement>> FetchUserAchievementByUserId(string userId)
    {
        await using var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString());
        await connection.OpenAsync();
        try
        {
            var query = await connection.QueryAsync<UserAchievementTable>
            (
                GetUserAchievementsSql,
                new { UserId = userId }
            );
            var userAchievementTables = query.ToList();
            if (!userAchievementTables.Any())
            {
                return new List<Achievement>();
            }

            var userAchievements = userAchievementTables.Select(ac => new Achievement
                {
                    Name = ac.name,
                    Description = ac.description,
                    Icon = ac.icon,
                    ExpReward = ac.reward,
                    UnlockDate = ac.unlocked_date,
                    Progress = ac.progress
                })
                .ToList();
            _logger.LogInformation(
                $"{userAchievements.Count} achievements have been successfully queried for user with id {userId}");
            return userAchievements;
        }
        catch (Exception e)
        {
            _logger.LogError(
                $"Something went wrong while fetching achievements for user with id {userId}, {e.StackTrace}");
            throw new QueryUserAchievementsException($"Unable to query user achievements", e);
        }
    }

    private const string GetUserAchievementsSql =
        """
        SELECT ua.*, a.*, uap.progress
        FROM
            user_progress.achievement a
            LEFT JOIN user_progress.user_achievement ua on a.id = ua.achievement_id
            LEFT JOIN user_progress.unlockable_achievement_progress uap on a.id = uap.achievement_id
        WHERE uap.user_id = @UserId OR ua.user_id = @UserId
        """;
}