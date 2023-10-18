using Dapper;
using EventManagementService.Application.ScraperEvents.Model;
using EventManagementService.Domain.Models;
using EventManagementService.Infrastructure.AppSettings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;

namespace EventManagementService.Application.ScraperEvents.Repository;

public interface ISqlScraperEvents
{
    Task<List<Event>> GetEvents();
}

public class SqlScraperEvents : ISqlScraperEvents
{
    private readonly IOptions<ConnectionStrings> _options;
    private readonly ILogger<SqlScraperEvents> _logger;

    public SqlScraperEvents
    (
        IOptions<ConnectionStrings> options,
        ILogger<SqlScraperEvents> logger
    )
    {
        _options = options;
        _logger = logger;
    }

    public async Task<List<Event>> GetEvents()
    {
        using (var connection = new NpgsqlConnection(_options.Value.Postgres))
        {
            await connection.OpenAsync();
            const string sql = """SELECT * FROM public.Event""";
            var result = await connection.QueryAsync<EventTableModel>(sql);

            return result.Select(e => new Event
            {
                Description = e.description,
                Location = new Location
                {
                    City = "test",
                    Country = "Test",
                    HouseNumber = "Test",
                    PostalCode = "Test",
                    StreetNumber = "test",
                    Floor = "test"
                },
                Title = e.title,
                Url = e.url
            }).ToList();
        }
    }
}