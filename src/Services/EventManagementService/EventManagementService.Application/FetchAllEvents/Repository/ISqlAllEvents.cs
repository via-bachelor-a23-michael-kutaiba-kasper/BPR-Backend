using Dapper;
using EventManagementService.Domain.Models.Events;
using EventManagementService.Infrastructure.AppSettings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;

namespace EventManagementService.Application.FetchAllEvents.Repository;

public interface ISqlAllEvents
{
    Task<IReadOnlyCollection<Event>> GetAllEvents();
}

public class SqlAllEvents : ISqlAllEvents
{
    private readonly IOptions<ConnectionStrings> _options;
    private readonly ILogger<SqlAllEvents> _logger;

    public SqlAllEvents
    (
        IOptions<ConnectionStrings> options,
        ILogger<SqlAllEvents> logger
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
}