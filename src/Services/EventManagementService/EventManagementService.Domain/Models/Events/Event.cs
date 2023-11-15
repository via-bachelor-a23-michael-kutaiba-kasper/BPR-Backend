namespace EventManagementService.Domain.Models.Events;

public class Event
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
    public string HostId { get; set; }
    public int MaxNumberOfAttendees { get; set; }
    public string Url { get; set; }
    public string? Description { get; set; }
    public Location Location { get; set; }
    public string AccessCode { get; set; }
    public Category Category { get; set; }
    public IEnumerable<Keyword> Keywords { get; set; }
    public IEnumerable<string> Images { get; set; }
}