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
                    UnlockDate = ac.unlocked_date
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
        SELECT * FROM user_achievement ua
            JOIN achievement a on a.id = ua.achievement_id
                 WHERE user_id = @UserId;
        """;
}