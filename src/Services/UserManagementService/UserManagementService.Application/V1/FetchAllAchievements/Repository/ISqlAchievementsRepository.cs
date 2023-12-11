using Dapper;
using Microsoft.Extensions.Logging;
using Npgsql;
using UserManagementService.Application.V1.FetchAllAchievements.Exceptions;
using UserManagementService.Application.V1.FetchAllAchievements.Model;
using UserManagementService.Infrastructure;

namespace UserManagementService.Application.V1.FetchAllAchievements.Repository;

public interface ISqlAchievementsRepository
{
    Task<IReadOnlyCollection<AchievementTable>> FetchAllAchievementsInTheSystem();
}

public class SqlAchievementsRepository : ISqlAchievementsRepository
{
    private readonly ILogger<SqlAchievementsRepository> _logger;
    private readonly IConnectionStringManager _connectionStringManager;

    public SqlAchievementsRepository
    (
        ILogger<SqlAchievementsRepository> logger,
        IConnectionStringManager connectionStringManager
    )
    {
        _logger = logger;
        _connectionStringManager = connectionStringManager;
    }

    public async Task<IReadOnlyCollection<AchievementTable>> FetchAllAchievementsInTheSystem()
    {
        await using var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString());
        await connection.OpenAsync();
        try
        {
            var query = await connection.QueryAsync<AchievementTable>(GetAllAchievementsSql);
            var dbAchievements = query.ToList();
            return dbAchievements;
        }
        catch (Exception e)
        {
            _logger.LogError($"Cannot query achievements, {e.StackTrace}");
            throw new QueryAllAchievementsException("Unable to query all achievements", e);
        }
    }
    
    private const string GetAllAchievementsSql = 
        """
        SELECT * FROM user_progress.achievement;
        """;
}