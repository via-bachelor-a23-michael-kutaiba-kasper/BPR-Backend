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
    Task<IReadOnlyCollection<Event>> GetAllEvents();
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

    public async Task<IReadOnlyCollection<Event>> GetAllEvents()
    {
        _logger.LogInformation("Fetching all public events from database");
        var events = new List<Event>();
        using (var connection = new NpgsqlConnection(_options.Value.Postgres))
        {
            await connection.OpenAsync();
            const string sql = """SELECT * FROM public.event""";
            var result = await connection.QueryAsync<EventTableModel>(sql);
            
            _logger.LogInformation($"{result.Count()} retrieved from database");

            /*events.AddRange(from e in result
            let l = JsonSerializer.Deserialize<Location>(e.location, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!
            select new Event
            {
                Title = e.title,
                Url = e.url,
                Description = e.description,
                Location = new Location
                {
                    Country = l.Country,
                    HouseNumber = l.HouseNumber,
                    PostalCode = l.PostalCode,
                    City = l.City,
                    StreetNumber = l.StreetNumber,
                    StreetName = l.StreetName,
                    Floor = l.Floor,
                    GeoLocation = new GeoLocation { Lat = l.GeoLocation.Lat, Lng = l.GeoLocation.Lng }
                }
            });*/

            return events;
        }
    }

    public async Task UpsertEvents(IReadOnlyCollection<Event> events)
    {
        _logger.LogInformation("Upserting public events");
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
                    _logger.LogInformation($"Location ->: {JsonSerializer.Serialize(item.Location)}");
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