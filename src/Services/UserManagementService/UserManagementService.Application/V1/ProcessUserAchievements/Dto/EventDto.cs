namespace UserManagementService.Application.V1.ProcessUserAchievements.Dto;

public class EventDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset LastUpdateDate { get; set; }
    public bool IsPrivate { get; set; }
    public bool AdultsOnly { get; set; }
    public bool IsPaid { get; set; }
    public UserDto Host { get; set; }
    public int MaxNumberOfAttendees { get; set; }
    public string Url { get; set; }
    public string? Description { get; set; }
    public string Location { get; set; }
    public string City { get; set; }
    public GeolocationDto GeoLocation { get; set; }
    public string AccessCode { get; set; }
    public string Category { get; set; }
    public IEnumerable<string> Keywords { get; set; }
    public IEnumerable<string>? Images { get; set; }
    public IEnumerable<UserDto>? Attendees { get; set; }
}

public class GeolocationDto
{
    public float Lat { get; set; }
    public float Lng { get; set; }
}

public class UserDto
{
    public string UserId { get; set; }
    public string? DisplayName { get; set; }
    public string? PhotoUrl { get; set; }
    public DateTimeOffset? LastSeenOnline { get; set; }
    public DateTimeOffset CreationDate { get; set; }
}