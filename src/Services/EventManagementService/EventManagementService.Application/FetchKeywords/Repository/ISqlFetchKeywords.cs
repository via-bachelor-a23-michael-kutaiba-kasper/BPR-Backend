using Dapper;
using EventManagementService.Application.FetchKeywords.Exceptions;
using EventManagementService.Infrastructure.AppSettings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;

namespace EventManagementService.Application.FetchKeywords.Repository;

public interface ISqlFetchKeywords
{
    Task<IReadOnlyCollection<string>> FetchKeywords();
}

public class SqlFetchKeywords : ISqlFetchKeywords
{
    private readonly ILogger<SqlFetchKeywords> _logger;
    private readonly IOptions<ConnectionStrings> _options;

    public SqlFetchKeywords
    (
        ILogger<SqlFetchKeywords> logger,
        IOptions<ConnectionStrings> options
    )
    {
        _logger = logger;
        _options = options;
    }

    public async Task<IReadOnlyCollection<string>> FetchKeywords()
    {
        await using var connection = new NpgsqlConnection(_options.Value.Postgres);
        try
        {
            await connection.OpenAsync();
            var query = await connection.QueryAsync<string>(GetAllKeywordsSql) as IReadOnlyCollection<string>;
            if (query == null) throw new NoKeywordsInDatabaseException("There are no keywords in the database");
            return query;
        }
        catch (Exception e)
        {
            _logger.LogCritical("Cannot fetch keywords from the database");
            throw new CannotFetchKeywords("Unable to fetch keywords", e);
        }
    }

    private const string GetAllKeywordsSql =
        """
        SELECT name FROM keyword;
        """;
}