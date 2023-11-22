using EventManagementService.Domain.Models.Events;

namespace EventManagementService.API.Controllers.V1.Dtos;

public class EventDto
{
    public string Title { get; set; } = default!;
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset LastUpdateDate { get; set; }
    public bool IsPrivate { get; set; }
    public bool AdultsOnly { get; set; }
    public bool IsPaid { get; set; }
    public string HostId { get; set; } = default!;
    public int MaxNumberOfAttendees { get; set; }
    public string? Description { get; set; }
    public LocationDto Location { get; set; }= default!;
    public string Category { get; set; } = default!;
    public IEnumerable<string> Keywords { get; set; }= default!;
}