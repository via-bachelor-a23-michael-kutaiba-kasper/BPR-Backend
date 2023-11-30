namespace EventManagementService.Application.FetchAllEvents.Model;

public class Filters
{
    public DateTimeOffset? From{ get; set; }
    public DateTimeOffset? To { get; set; }
    public string? HostId { get; set; }
    public bool IncludePrivateEvents { get; set; } = false;
}