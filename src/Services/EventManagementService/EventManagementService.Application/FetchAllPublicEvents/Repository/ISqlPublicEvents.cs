using System.Text.Json;
using Dapper;
using EventManagementService.Application.FetchAllPublicEvents.Exceptions;
using EventManagementService.Application.FetchAllPublicEvents.Model;
using EventManagementService.Domain.Models.Events;
using EventManagementService.Infrastructure.AppSettings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;

namespace EventManagementService.Application.FetchAllPublicEvents.Repository;

public interface ISqlPublicEvents
{
    Task<List<Event>> GetEvents();
    Task UpsertEvents(IReadOnlyCollection<Event> events);
}

public class SqlPublicEvents : ISqlPublicEvents
{
    private readonly IOptions<ConnectionStrings> _options;
    private readonly ILogger<SqlPublicEvents> _logger;

    public SqlPublicEvents
    (
        IOptions<ConnectionStrings> options,
        ILogger<SqlPublicEvents> logger
    )
    {
        _options = options;
        _logger = logger;
    }

    public async Task<List<Event>> GetEvents()
    {
        using (var connection = new NpgsqlConnection(_options.Value.PostgresLocal))
        {
            await connection.OpenAsync();
            const string sql = """SELECT * FROM public.event""";
            var result = await connection.QueryAsync<EventTableModel>(sql);

            return result.Select(e => new Event
            {
                Description = e.description,
                Location = JsonSerializer.Deserialize<Location>(e.location,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!,
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

            using (var connection = new NpgsqlConnection(_options.Value.PostgresLocal))
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