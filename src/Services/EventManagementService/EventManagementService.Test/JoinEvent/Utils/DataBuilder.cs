using EventManagementService.Domain.Models.Events;
using EventManagementService.Infrastructure;
using Npgsql;

namespace EventManagementService.Test.JoinEvent.Utils;

public class DataBuilder
{
    private readonly ConnectionStringManager _connectionStringManager;

    public DataBuilder(ConnectionStringManager connectionStringManager)
    {
        _connectionStringManager = connectionStringManager;
    }
    
    public Event CreateTestEvent(Action<Event>? configureEvent = null)
    {
        Event newEvent = new()
        {
            Id = 1,
            Location = new Location
            {
                City = "Horsens",
                Country = "Denmark",
                PostalCode = "8700",
                StreetName = "Vejlevej",
                StreetNumber = "14",
                GeoLocation = new GeoLocation
                {
                    Lat = 0,
                    Lng = 0
                }
            },
            Category = new Category { Id = 1, Name = "Music" },
            Images = new List<string>(),
            Title = "Beethoven Concerto",
            Keywords = new List<Keyword>() { new Keyword { Id = 1, Name = "Classic Music" } },
            Url = "http://test.com/events/1",
            AdultsOnly = false,
            CreatedDate = DateTimeOffset.Now,
            StartDate = DateTimeOffset.Now.AddDays(1),
            EndDate = DateTimeOffset.Now.AddDays(1).AddHours(2),
            AccessCode = "321km3lkmdkslajdkas321",
            HostId = "Oq8tmHrYV6SeEpWf1olCJNJ1JW93",
            IsPaid = true,
            IsPrivate = false,
            MaxNumberOfAttendees = 200,
            LastUpdateDate = DateTimeOffset.Now,
        };

        configureEvent?.Invoke(newEvent);
        
        using (var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString()))
        {
            connection.Open();    
            
        }
        
        return newEvent;
    }
}