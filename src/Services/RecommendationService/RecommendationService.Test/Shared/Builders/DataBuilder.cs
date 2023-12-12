using Dapper;
using EventManagementService.Domain.Models;
using EventManagementService.Domain.Models.Events;
using Npgsql;
using RecommendationService.Domain;
using RecommendationService.Domain.Events;
using RecommendationService.Infrastructure;

namespace RecommendationService.Test.Shared.Builders;

public class DataBuilder
{
    private int nextId = 1;
    private readonly ConnectionStringManager _connectionStringManager;
    public IList<Event> EventSet { get; set; } = new List<Event>();

    public DataBuilder(ConnectionStringManager connectionStringManager)
    {
        _connectionStringManager = connectionStringManager;
    }

    public DataBuilder InsertEvents(IReadOnlyCollection<Event> events)
    {
        using (var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString()))
        {
            connection.Open();
            foreach (var e in events)
            {
                var statement = SqlStatements.InsertEvent(e);
                var id = connection.ExecuteScalar<int>(statement);
                e.Id = id;

                foreach (var keyword in e.Keywords)
                {
                    connection.Execute(
                        "INSERT INTO event_keyword(event_id, keyword) VALUES (@eventId, @keyword)",
                        new {@eventId = e.Id, @keyword = keyword});
                }

                foreach (var attendee in e.Attendees)
                {
                    connection.Execute(
                        "INSERT INTO event_attendee(event_id, user_id) VALUES (@eventId, @userId)",
                        new {@eventId = e.Id, @userId = attendee.UserId});
                }

                foreach (var image in e.Images)
                {
                    connection.Execute(
                        "INSERT INTO image(event_id, uri) VALUES (event_id=@eventId, uri=@uri)",
                        new {@eventId = e.Id, @uri = image});
                }

                EventSet.Add(e);
            }

            connection.Close();
        }

        return this;
    }

    public Event BuildTestEventObject(Action<Event>? configureEvent = null)
    {
        Event newEvent = new()
        {
            Id = nextId,
            Category = Category.Music,
            Images = new List<string>(),
            Title = "Beethoven Concerto",
            Keywords = new List<Keyword> {Keyword.ClassicalPerformance},
            Url = "http://test.com/events/1",
            AdultsOnly = false,
            CreatedDate = DateTimeOffset.UtcNow,
            StartDate = DateTimeOffset.UtcNow.AddDays(1),
            EndDate = DateTimeOffset.UtcNow.AddDays(1).AddHours(2),
            AccessCode = "$UNPROCESSED$",
            Host = new User
            {
                UserId = "Oq8tmHrYV6SeEpWf1olCJNJ1JW93"
            },
            IsPaid = true,
            IsPrivate = false,
            MaxNumberOfAttendees = 200,
            LastUpdateDate = DateTimeOffset.UtcNow,
            Location = "Vejlevej 14, 8700 Horsens, Denmark",
            City = "Horsens",
            GeoLocation = new GeoLocation
            {
                Lat = 0,
                Lng = 0
            },
            Attendees = new List<User>()
        };

        configureEvent?.Invoke(newEvent);
        newEvent.AccessCode = newEvent.AccessCode == "$UNPROCESSED$" ? UniqueEventAccessCodeGenerator.GenerateUniqueString(newEvent.Title, newEvent.CreatedDate) : newEvent.AccessCode;
        nextId++;

        return newEvent;
    }
}