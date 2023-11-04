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
            var result = await connection.QueryAsync(GetEventsSql());

            events.AddRange(result.Select(e => new Event
            {
                Title = e.title,
                Description = e.description,
                Url = e.url,
                Location = new Location
                {
                    Country = e.country,
                    StreetName = e.street_name,
                    StreetNumber = e.street_number,
                    HouseNumber = e.sub_premise,
                    PostalCode = e.postal_code,
                    City = e.city,
                    GeoLocation = new GeoLocation { Lat = e.geolocation_lat, Lng = e.geolocation_lng }
                }
            }));

            _logger.LogInformation($"{result.Count()} retrieved from database");

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

    private static string GetEventsSql()
    {
        return """
               SELECT 
                   e.*,
                   l.*
               FROM 
                   postgres.public.event e
               JOIN 
                       location l ON e.location_id = l.id
               LEFT JOIN 
                       public.event_attendee ea on e.id = ea.event_id
               LEFT JOIN 
                       event_category ec ON e.id = ec.event_id
               LEFT JOIN 
                       category c ON ec.category_id = c.id
               LEFT JOIN 
                       keyword_category kc ON e.id = kc.event_id
               LEFT JOIN 
                       keyword k ON kc.keyword = k.id;
               """;
    }

    private static string InsertEventSql()
    {
        //TODO: update this insert -> look into temp tables to insert and then use merge to copy data using binary copy
        return """
               INSERT INTO public.event(title,url,location,description)
               values (@title, @url, @location, @description)
               """;
    }
}