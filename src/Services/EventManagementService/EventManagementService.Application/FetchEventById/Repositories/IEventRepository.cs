using Dapper;
using EventManagementService.Application.FetchEventById.Data;
using EventManagementService.Domain.Models.Events;
using EventManagementService.Infrastructure;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace EventManagementService.Application.FetchEventById.Repositories;

public interface IEventRepository
{
    public Task<Event?> GetEventByIdAsync(int id);
}

public class EventRepository : IEventRepository
{
    private readonly ILogger<EventRepository> _logger;
    private readonly IConnectionStringManager _connectionStringManager;
    
    public EventRepository(ILogger<EventRepository> logger, IConnectionStringManager connectionStringManager)
    {
        _logger = logger;
        _connectionStringManager = connectionStringManager;
    }
    
    public async Task<Event?> GetEventByIdAsync(int id)
    {
        using (var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString()))
        {
            await connection.OpenAsync();
            var queryParams = new { @eventId = id };
            
            var eventEntity = await connection.QueryFirstOrDefaultAsync<EventEntity>(SqlQueries.QueryAllFromEventTableByEventId, queryParams);
            
            if (eventEntity is null)
            {
                return null;
            }
            
            var attendees = await connection.QueryAsync<string>(SqlQueries.QueryEventAttendees, queryParams);
            var keywords = (await connection.QueryAsync<int>(SqlQueries.QueryEventKeywords, queryParams)).Select(kw => (Keyword) kw);
            var images = await connection.QueryAsync<string>(SqlQueries.QueryEventImages, queryParams);
            
            Event existingEvent = new()
            {
                Description = eventEntity.description,
                Category = (Category)eventEntity.category_id,
                AccessCode = eventEntity.access_code,
                AdultsOnly = eventEntity.adult_only,
                CreatedDate = eventEntity.created_date,
                Keywords = keywords,
                EndDate = eventEntity.end_date,
                IsPrivate = eventEntity.is_private,
                StartDate = eventEntity.start_date,
                LastUpdateDate = eventEntity.last_update_date,
                MaxNumberOfAttendees = eventEntity.max_number_of_attendees,
                HostId = eventEntity.host_id,
                IsPaid = eventEntity.is_paid,
                Images = images,
                Title = eventEntity.title,
                Url = eventEntity.url,
                Attendees = attendees,
                Id = eventEntity.id,
                Location = new Location()
                {
                    Id = eventEntity.location_id,
                    City = eventEntity.city,
                    Country = eventEntity.country,
                    SubPremise = eventEntity.sub_premise,
                    GeoLocation = new GeoLocation()
                    {
                        Lat = eventEntity.geolocation_lat,
                        Lng = eventEntity.geolocation_lng
                    },
                    PostalCode = eventEntity.postal_code,
                    StreetName = eventEntity.street_name,
                    StreetNumber = eventEntity.street_number
                }
            };
            await connection.CloseAsync();
            return existingEvent;
        }
    }
}