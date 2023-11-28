using Dapper;
using EventManagementService.Domain.Models;
using EventManagementService.Domain.Models.Events;
using EventManagementService.Infrastructure;
using Npgsql;

namespace EventManagementService.Test.JoinEvent.Utils;

public class DataBuilder
{
    /*private readonly ConnectionStringManager _connectionStringManager;
    public IList<Location> LocationsSet { get; set; } = new List<Location>();
    public IList<Event> EventSet { get; set; } = new List<Event>();

    public DataBuilder(ConnectionStringManager connectionStringManager)
    {
        _connectionStringManager = connectionStringManager;
    }
    
    public DataBuilder CreateLocations(IReadOnlyCollection<Location> locations)
    {
        using (var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString()))
        {
            connection.Open();
            foreach (var location in locations)
            {
                var statement = SqlStatements.InsertLocation(location);
                var id = connection.ExecuteScalar<int>(statement);
                location.Id = id;
                LocationsSet.Add(location);
            }
            connection.Close();
        }

        return this;
    }

    public DataBuilder CreateEvents(IReadOnlyCollection<Event> events)
    {
        using (var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString()))
        {
            connection.Open();
            foreach (var e in events)
            {
                var statement = SqlStatements.InsertEvent(e);
                var id = connection.ExecuteScalar<int>(statement);
                e.Id = id;
                EventSet.Add(e);
            }
            connection.Close();
        }

        return this;
    }
    
    public Event NewTestEvent(Action<Event>? configureEvent = null)
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
            Category = Category.Music,
            Images = new List<string>(),
            Title = "Beethoven Concerto",
            Keywords = new List<Keyword> { Keyword.ClassicalPerformance },
            Url = "http://test.com/events/1",
            AdultsOnly = false,
            CreatedDate = DateTimeOffset.UtcNow,
            StartDate = DateTimeOffset.UtcNow.AddDays(1),
            EndDate = DateTimeOffset.UtcNow.AddDays(1).AddHours(2),
            AccessCode = "321km3lkmdkslajdkas321",
            Host = new User
            {
                UserId = "Oq8tmHrYV6SeEpWf1olCJNJ1JW93"
            },
            IsPaid = true,
            IsPrivate = false,
            MaxNumberOfAttendees = 200,
            LastUpdateDate = DateTimeOffset.UtcNow,
        };

        configureEvent?.Invoke(newEvent);

        using (var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString()))
        {
            connection.Open();
        }

        return newEvent;
    }*/
}