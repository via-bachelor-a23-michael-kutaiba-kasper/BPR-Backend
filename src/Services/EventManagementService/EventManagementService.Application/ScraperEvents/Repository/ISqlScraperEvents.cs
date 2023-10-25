using System.Text.Json;
using Dapper;
using EventManagementService.Application.ScraperEvents.Exceptions;
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
    Task UpsertEvents(IReadOnlyCollection<Event> events);
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

    public async Task UpsertEvents(IReadOnlyCollection<Event> events)
    {
        try
        {
            var command = InsertEventSql();

            using (var connection = new NpgsqlConnection(_options.Value.Postgres))
            {
                await connection.OpenAsync();
                foreach (var item in events)
                {
                    var parameters = new
                    {
                        @title = item.Title, 
                        @url = item.Url, 
                        @description = item.Description,
                        @location = JsonSerializer.Serialize(item.Location)
                    };

                    connection.Execute(command, parameters);
                }
            }
        }
        catch (Exception e)
        {
            throw new UpsertScraperEventsException($"Cannot insert or update scraper events: {e.Message}", e);
        }
    }

    private static string InsertEventSql()
    {
        return """
               INSERT INTO public.event(title,url,location,description)
               values (@title, @url, @location, @description)
               """;
    }
}