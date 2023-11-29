namespace EventManagementService.API.Controllers.V1.EventControllers.Dtos;

public class EventDto
{
    public int Id{ get; set; }
    public string Title { get; set; } = default!;
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset LastUpdateDate { get; set; }
    public bool IsPrivate { get; set; }
    public bool AdultsOnly { get; set; }
    public bool IsPaid { get; set; }
    public UserDto Host { get; set; }
    public int MaxNumberOfAttendees { get; set; }
    public string? Description { get; set; }
    public string Location { get; set; }
    public string City { get; set; }
    public GeoLocationDto GeoLocation { get; set; }= default!;
    public string Category { get; set; } = default!;
    public IEnumerable<string> Keywords { get; set; } = default!;
}