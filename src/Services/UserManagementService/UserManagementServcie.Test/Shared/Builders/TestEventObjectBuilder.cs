
using UserManagementService.Domain.Models;
using UserManagementService.Domain.Models.Events;

namespace UserManagementServcie.Test.Shared.Builders;

public class TestEventObjectBuilder
{
    private int nextId = 1;
    public IList<Event> EventSet { get; set; } = new List<Event>();

    public TestEventObjectBuilder()
    {
    }

    public Event NewTestEvent(Action<Event>? configureEvent = null)
    {
        var newEvent = new Event()
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