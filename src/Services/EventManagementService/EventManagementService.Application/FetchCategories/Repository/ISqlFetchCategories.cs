using Dapper;
using EventManagementService.Application.FetchCategories.Exceptions;
using EventManagementService.Domain.Models.Events;
using EventManagementService.Infrastructure.AppSettings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;

namespace EventManagementService.Application.FetchCategories.Repository;

public interface ISqlFetchCategories
{
    Task<IReadOnlyCollection<string>> GetCategories();
}

public class SqlFetchCategories : ISqlFetchCategories
{
    private readonly ILogger<SqlFetchCategories> _logger;
    private readonly IOptions<ConnectionStrings> _options;

    public SqlFetchCategories
    (
        ILogger<SqlFetchCategories> logger,
        IOptions<ConnectionStrings> options
    )
    {
        _logger = logger;
        _options = options;
    }

    public async Task<IReadOnlyCollection<string>> GetCategories()
    {
        await using var connection = new NpgsqlConnection(_options.Value.Postgres);
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
        SELECT name FROM category;
        """;
}