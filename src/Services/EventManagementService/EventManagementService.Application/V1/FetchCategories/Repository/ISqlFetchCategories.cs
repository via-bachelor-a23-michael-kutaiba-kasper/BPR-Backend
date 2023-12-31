using Dapper;
using EventManagementService.Application.V1.FetchCategories.Exceptions;
using EventManagementService.Infrastructure;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace EventManagementService.Application.V1.FetchCategories.Repository;

public interface ISqlFetchCategories
{
    Task<IReadOnlyCollection<string>> GetCategories();
}

public class SqlFetchCategories : ISqlFetchCategories
{
    private readonly ILogger<SqlFetchCategories> _logger;
    private readonly IConnectionStringManager _connectionStringManager;

    public SqlFetchCategories
    (
        ILogger<SqlFetchCategories> logger,
        IConnectionStringManager connectionStringManager
    )
    {
        _logger = logger;
        _connectionStringManager = connectionStringManager;
    }

    public async Task<IReadOnlyCollection<string>> GetCategories()
    {
        await using var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString());
        try
        {
            await connection.OpenAsync();
            var query = await connection.QueryAsync<string>(GetAllCategoriesSql) as IReadOnlyCollection<string>;
            if (query == null) throw new NoCategoriesInDatabaseException("There are no categories in the database");
            return query;
        }
        catch (Exception e)
        {
            _logger.LogCritical("Cannot fetch categories from the database");
            throw new CannotFetchCategories("Unable to fetch categories", e);
        }
    }

    private const string GetAllCategoriesSql =
        """
        SELECT name FROM category WHERE name <> 'Un Assigned';
        """;
}