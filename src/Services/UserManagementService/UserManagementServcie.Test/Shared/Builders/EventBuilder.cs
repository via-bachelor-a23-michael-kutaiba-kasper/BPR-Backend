using UserManagementService.Domain.Models;
using UserManagementService.Domain.Models.Events;

namespace UserManagementServcie.Test.Shared.Builders;

public class EventBuilder
{
    private static readonly Random Random = new Random();

    private Event _event = new Event();

    public EventBuilder WithRequiredFields
    (
        string title,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        DateTimeOffset createdDate,
        bool isPrivate,
        bool adultOnly,
        bool isPaied,
        string hostId,
        string accessCode,
        Category categoryId,
        string location,
        string city,
        float geoLocationLat,
        float geoLocationLng
    )
    {
        _event.Title = title;
        _event.StartDate = startDate;
        _event.EndDate = endDate;
        _event.CreatedDate = createdDate;
        _event.IsPrivate = isPrivate;
        _event.AdultsOnly = adultOnly;
        _event.IsPaid = isPaied;
        _event.Host = new User
        {
            UserId = hostId
        };
        _event.AccessCode = accessCode;
        _event.Category = categoryId;
        _event.Location = location;
        _event.City = city;
        _event.GeoLocation = new GeoLocation
        {
            Lat = geoLocationLat,
            Lng = geoLocationLng
        };
        return this;
    }

    public EventBuilder WithMaxNumberOfAttendees(int value)
    {
        _event.MaxNumberOfAttendees = value;
        return this;
    }

    public EventBuilder WithLastUpdateDate(DateTimeOffset value)
    {
        _event.LastUpdateDate = value;
        return this;
    }

    public EventBuilder WithUrl(string value)
    {
        _event.Url = value;
        return this;
    }

    public EventBuilder WithDescription(string value)
    {
        _event.Description = value;
        return this;
    }

    public Event Build()
    {
        if (_event.Title == null)
            throw new InvalidOperationException("Title is required.");

        if (_event.StartDate == default)
            throw new InvalidOperationException("StartDate is required.");

        if (_event.EndDate == default)
            throw new InvalidOperationException("EndDate is required.");
        
        if (_event.Location == null)
            throw new InvalidOperationException("Location is required.");

        if (_event.AccessCode == null)
            throw new InvalidOperationException("AccessCode is required.");

        if (_event.Host?.UserId == null)
            throw new InvalidOperationException("HostId is required.");

        if (_event.Category == Category.UnAssigned)
            throw new InvalidOperationException("Category is required.");

        if (_event.GeoLocation == null)
            throw new InvalidOperationException("GeoLocation is required.");
        if (_event.GeoLocation.Lat is < -90.0f or > 90.0f)
            throw new InvalidOperationException("GeoLocationLat is invalid -> It should be between or equal to -90 and 90");
        if (_event.GeoLocation.Lng is < -180.0f or > 180.0f)
            throw new InvalidOperationException("GeoLocationLat is invalid -> It should be between or equal to -180 and 180");
        
        
        _event.Description ??= string.Empty;
        _event.City ??= string.Empty;
        _event.Images ??= new List<string>();
        _event.Attendees ??= new List<User>();

        return _event;
    }
}