using Dapper;
using Microsoft.Extensions.Logging;
using Npgsql;
using UserManagementService.Application.V1.GetUserExpProgres.Data;
using UserManagementService.Application.V1.GetUserExpProgres.Exceptions;
using UserManagementService.Domain.Models;
using UserManagementService.Infrastructure;

namespace UserManagementService.Application.V1.GetUserExpProgres.Repository;

public interface ILevelRepository
{
    Task<IReadOnlyCollection<Level>> GetAllLevelsAsync();
}

public class LevelRepository : ILevelRepository
{
    private readonly ILogger<LevelRepository> _logger;
    private readonly IConnectionStringManager _connectionStringManager;

    public LevelRepository(ILogger<LevelRepository> logger, IConnectionStringManager connectionStringManager)
    {
        _logger = logger;
        _connectionStringManager = connectionStringManager;
    }

    public async Task<IReadOnlyCollection<Level>> GetAllLevelsAsync()
    {
        try
        {
            await using var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString());
            await connection.OpenAsync();

            const string query = "SELECT * FROM user_progress.level";
            var entities = await connection.QueryAsync<LevelEntity>(query);

            return entities.Select(entity => new Level
            {
                Name = entity.name,
                Value = entity.id,
                MaxExp = entity.max_exp,
                MinExp = entity.min_exp
            }).ToList();
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to retrieve levels from postgres db");
            _logger.LogError(e.Message);
            _logger.LogError(e.StackTrace);
            throw new UnableToRetrieveLevelsException(e);
        }
    }
}