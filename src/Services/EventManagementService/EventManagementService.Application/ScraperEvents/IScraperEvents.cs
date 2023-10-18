using Dapper;
using EventManagementService.Domain.Models;
using EventManagementService.Infrastructure.AppSettings;
using Microsoft.Extensions.Options;
using Npgsql;

namespace EventManagementService.Application.ScraperEvents;

public interface IScraperEvents
{
    Task<List<Event>> GetEvents();
}

public class ScraperEvents : IScraperEvents
{
    private readonly IOptions<ConnectionStrings> _options;

    public ScraperEvents(IOptions<ConnectionStrings> options)
    {
        _options = options;
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