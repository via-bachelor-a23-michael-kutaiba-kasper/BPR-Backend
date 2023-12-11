using Dapper;
using Microsoft.Extensions.Logging;
using Npgsql;
using UserManagementService.Application.V1.GetUserExpProgres.Exceptions;
using UserManagementService.Application.V1.ProcessExpProgress.Data;
using UserManagementService.Domain.Models;
using UserManagementService.Infrastructure;

namespace UserManagementService.Application.V1.GetUserExpProgres.Repository;

public interface IProgressRepository
{
    Task<Progress?> GetUserExpProgressAsync(string userId);
}

public class ProgressRepository : IProgressRepository
{
    private readonly ILogger<ProgressRepository> _logger;
    private readonly IConnectionStringManager _connectionStringManager;

    public ProgressRepository(ILogger<ProgressRepository> logger, IConnectionStringManager connectionStringManager)
    {
        _logger = logger;
        _connectionStringManager = connectionStringManager;
    }

    public async Task<Progress?> GetUserExpProgressAsync(string userId)
    {
        try
        {
            await EnsureUserHasProgress(userId);
            var progressEntity = await GetProgressEntity(userId);
            var history = await GetExpProgressEntities(userId);

            if (progressEntity is null)
            {
                return null;
            }

            return new Progress
            {
                Id = progressEntity.id,
                Level = new Level(),
                Unlockables = new List<Unlockable>(),
                TotalExp = progressEntity.total_exp,
                ExpProgressHistory = history.Select(entity => new ExpProgressEntry()
                    {ExpGained = entity.exp_gained, Timestamp = entity.datetime}).ToList()
            };
        }
        catch (Exception e)
        {
            _logger.LogError("Unable to retrieve progress from postgres db");
            _logger.LogError(e.Message);
            _logger.LogError(e.StackTrace);
            throw new UnableToRetrieveUserExpProgressException(userId);
        }
    }

    private async Task<IEnumerable<UserExpProgressEntity>> GetExpProgressEntities(string userId)
    {
        await using var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString());
        await connection.OpenAsync();

        var getProgressQuery =
            "SELECT * FROM user_progress.user_exp_progress WHERE user_id=@userId ORDER BY datetime ASC";
        var entities = await connection.QueryAsync<UserExpProgressEntity>(getProgressQuery, new
        {
            @userId = userId
        });

        await connection.CloseAsync();

        return entities;
    }

    private async Task<ProgressEntity?> GetProgressEntity(string userId)
    {
        await using var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString());
        await connection.OpenAsync();

        var getProgressQuery = "SELECT * FROM user_progress.progress WHERE user_id = @userId";
        var entity = await connection.QueryFirstOrDefaultAsync<ProgressEntity>(getProgressQuery, new
        {
            @userId = userId
        });

        await connection.CloseAsync();

        return entity;
    }

    private async Task EnsureUserHasProgress(string userId)
    {
        await using var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString());
        await connection.OpenAsync();

        var existingEntity = await GetProgressEntity(userId);

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
}