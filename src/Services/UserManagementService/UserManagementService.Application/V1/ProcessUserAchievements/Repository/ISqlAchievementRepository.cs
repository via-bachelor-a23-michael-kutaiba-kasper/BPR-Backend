using System.Transactions;
using Dapper;
using Microsoft.Extensions.Logging;
using Npgsql;
using UserManagementService.Application.V1.ProcessUserAchievements.Exceptions;
using UserManagementService.Application.V1.ProcessUserAchievements.Mapper;
using UserManagementService.Application.V1.ProcessUserAchievements.Model;
using UserManagementService.Domain.Models;
using UserManagementService.Infrastructure;

namespace UserManagementService.Application.V1.ProcessUserAchievements.Repository;

public interface ISqlAchievementRepository
{
    Task InsertUserAchievement(IReadOnlyCollection<UserAchievementTable> userAchievements);
    Task<IReadOnlyCollection<UserAchievementJoinTable>?> GetUserAchievement(string userId);
    Task<IReadOnlyCollection<AchievementTable>> GetAchievements();
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

    public async Task InsertUserAchievement(IReadOnlyCollection<UserAchievementTable> userAchievements)
    {
        await using var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString());
        await connection.OpenAsync();
        await using NpgsqlTransaction transaction = await connection.BeginTransactionAsync();
        try
        {
            await connection.ExecuteAsync(InsertNewUserAchievementSql, userAchievements);
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
}